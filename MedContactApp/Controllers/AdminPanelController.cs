using AutoMapper;
using MedContactApp.AdminPanelHelpers;
using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactApp.Models;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;
using System.Security.Claims;

namespace MedContactApp.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly AdminModelBuilder _adminModelBuilder;
        private readonly AdminSortFilter _adminSortFilter;
        private int _pageSize = 7;

        public AdminPanelController(IUserService userService,
            IMapper mapper, IRoleService roleService, IConfiguration configuration,
            AdminModelBuilder adminModelBuilder,
            AdminSortFilter adminSortFilter)
          
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
            _adminModelBuilder = adminModelBuilder;
            _adminSortFilter = adminSortFilter;
        }

        [HttpGet]
        public IActionResult AdminPanelMenu()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {
                var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                if (roles != null && roles.Any(r => r.Equals("Admin")))
                    return View();
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = "Admin", Policy = "FullBlocked")]
        public async Task<IActionResult> UserIndex(string email, string name, string surname, string role, int page = 1,
               SortState sortOrder = SortState.EmailAsc, GroupState groupOrder = GroupState.FamilyGroupOff)

        {
            string icon = string.Empty;
            var res = Guid.TryParse(role, out Guid roleId);
            if (!res)
                roleId = default;
            try
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                IQueryable<User> users = _userService.GetUsers().Include(u => u.Roles);

                if (groupOrder.Equals(GroupState.FamilyGroupOn))
                {
                    users = users.Include(u => u.Family);
                }
              
                var roles = _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToList();

                if (roleId != default)
                {
                    users = (from u in users
                             from r in u.Roles!
                             where r.Id.Equals(roleId)
                             select u);
                }


                users = _adminSortFilter.UserFilter(users, email, name, surname);

                if (groupOrder.Equals(GroupState.FamilyGroupOn))
                {
                    users = _adminSortFilter.UserSort(users, sortOrder, 1);
                    icon = @$"<i class=""bi bi-check2-circle""></i>";
                }
                else
                {
                    users = _adminSortFilter.UserSort(users, sortOrder);
                }

                var count = await users.CountAsync();
                var items = await users.Skip((page - 1) * pageSize).Take(pageSize)
                           .Select(user => _mapper.Map<UserDto>(user)).ToListAsync();

                string pageRoute = @"/adminpanel/userindex?page=";
                string processOptions = $"&grouporder={groupOrder}&email{email}&name={name}&roleid={roleId}&sortorder={sortOrder}";

                string link= Request.Path.Value + Request.QueryString.Value ;
                link = link.Replace("&", "*");
                ViewData["Reflink"] = link;

                UserIndexViewModel viewModel = new(
                    items, processOptions, groupOrder, icon,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterViewModel(roles, roleId, name, email),
                    new SortViewModel(sortOrder)
                );
                return View(viewModel);

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin", Policy = "FullBlocked")]
        public async Task<IActionResult> UserDetails(string? id, string? reflink = "")
        {
           try
           { 
            AdminUserEditModel emptyModel = new();
            var model = await _adminModelBuilder.AdminUserModelBuildAsync(emptyModel, id);
            if (model == null)
                //return NotFound();
                return NotFound("User is not found");
            if (!string.IsNullOrEmpty(reflink))
                model.Reflink = reflink.Replace("*", "&");
            else
                model.Reflink = reflink;

            return View(model);
           }
           catch (Exception e)
           {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
           }
        }

        [HttpPost]
        [Authorize(Roles = "Admin", Policy = "FullBlocked")]
        public async Task<IActionResult> UserDetails(AdminUserEditModel model)
        {
            if (model == null || model.Id == null)
                //return BadRequest();
                return new BadRequestObjectResult("Model is null");
            int addresult = 0;
            int subtract = 0;
            int changeRes = 0;

            try
            {
                var modelFull = await _adminModelBuilder.AdminUserModelBuildAsync(model, String.Empty);
                if (modelFull == null) //throw new Exception();
                    return new BadRequestObjectResult("User data was not received");

                if (modelFull != null && modelFull.BlockStateIds != null)
                {
                    foreach (var item in modelFull.BlockStateIds)
                    {
                        if (item == 1)
                        {
                            modelFull!.IsFullBlocked = false;
                            var sitem = modelFull?.BlockState?.First(x => x.IntId == 1);
                            sitem!.IsSelected = true;
                        }

                        if (item == 2)
                        {
                            modelFull!.IsFullBlocked = true;
                            var sitem = modelFull?.BlockState?.First(x => x.IntId == 1);
                            sitem!.IsSelected = true;
                        }

                    }
                    changeRes = await _userService.ChangeUserFullBlockById((Guid)model.Id, modelFull?.IsFullBlocked);
                }
                if (modelFull != null && modelFull.Id != null)
                {
                    if (modelFull.RoleIds != null)
                    {
                        foreach (var roleId in modelFull.RoleIds!)
                        {
                            var role = modelFull?.AllRoles?.FirstOrDefault(role => role.Id.Equals(roleId));

                            if (role != null)
                            {
                                role.IsSelected = true;
                                addresult += await _roleService.AddRoleByNameToUser((Guid)(modelFull?.Id!), role?.Name!);
                            }
                        }
                        foreach (var role in modelFull.AllRoles!)
                        {
                            if (modelFull!.RoleIds.All(r => r != role.Id))
                            {
                                role.IsSelected = false;
                                if (modelFull.RoleNames!.Any(x => x.Equals(role.Name)))
                                    subtract += await _roleService.RemoveRoleByNameFromUser((Guid)(modelFull?.Id!), role?.Name!);
                            }
                        }

                        modelFull.SystemInfo = $"<b>Roles:<br/>{addresult} was/were added<br/>{subtract} was/were deleted</b>";
                        if (changeRes > 0)
                        {
                            modelFull.SystemInfo += $"<br/><b>Full block status was changed</b>";
                        }
                        else
                        {
                            modelFull.SystemInfo += $"<br/><b>Full block status was not changed</b>";
                        }

                        return View(modelFull);
                    }

                    else
                    {
                        modelFull!.SystemInfo = $"<b>You have not chosen any role</b>";
                        return View(modelFull);
                    }
                }

                modelFull!.SystemInfo = "<b>Something went wrong (</b>";

                return View(modelFull);

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }


       
    }
}