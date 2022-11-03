using Microsoft.AspNetCore.Mvc;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactApp.Models;
using AutoMapper;
using Serilog;
using System.ComponentModel.Design;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using MedContactDb.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MedContactBusiness.ServicesImplementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using MedContactApp.Helpers;
using Microsoft.CodeAnalysis.Differencing;
using MedContactApp.FilterSortHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore.Query;
using MedContactApp.FilterSortPageHelpers;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Build.Framework;

namespace MedContactApp.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private int _pageSize = 7;

        public AdminPanelController(IUserService userService,
            IMapper mapper, IRoleService roleService, IConfiguration configuration, ModelUserBuilder modelBuilder)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
            _modelBuilder = modelBuilder;
        }

        [HttpGet]
        public  IActionResult AdminPanelMenu()
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
        public async Task <IActionResult> UserIndex(string email, string name, string surname, string role, int page = 1, 
               SortState sortOrder = SortState.EmailAsc)
           
        {
            
            var res = Guid.TryParse(role, out Guid roleId);
            if (!res)
                roleId = default;
            try 
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                IQueryable<User> users = _userService.GetUsers().Include(u => u.Roles);
                var roles = _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToList();
              
                if (roleId != default)
                {
                    users = (from u in users
                             from r in u.Roles!
                             where r.Id.Equals(roleId)
                             select u);
                }


                if (!string.IsNullOrEmpty(email))
                {
                    users = users.Where(p => p.Email!.Contains(email));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    users = users.Where(p => p.Name!.Contains(name) || p.MidName!.Contains(name));
                }
                if (!string.IsNullOrEmpty(surname))
                {
                    users = users.Where(p => p.Surname!.Contains(surname));
                }

              
                switch (sortOrder)
                {
                    case SortState.EmailDesc:
                        users = users.OrderByDescending(s => s.Email);
                        break;
                    case SortState.NameAsc:
                        users = users.OrderBy(s => s.Name);
                        break;
                    case SortState.NameDesc:
                        users = users.OrderByDescending(s => s.Name);
                        break;
                    case SortState.SurnameAsc:
                        users = users.OrderBy(s => s.Surname);
                        break;
                    case SortState.SurnameDesc:
                        users = users.OrderByDescending(s => s.Surname);
                        break;
                    case SortState.BirtDateAsc:
                        users = users.OrderBy(s => s.BirthDate);
                        break;
                    case SortState.BirthDateDesc:
                        users = users.OrderByDescending(s => s.BirthDate);
                        break;
                    case SortState.LastLoginAsc:
                        users = users.OrderBy(s => s.LastLogin);
                        break;
                    case SortState.LastLoginDesc:
                        users = users.OrderByDescending(s => s.LastLogin);
                        break;
                    case SortState.IsFullBlockedAsc:
                        users = users.OrderBy(s => s.IsFullBlocked);
                        break;
                    case SortState.IsFullBlockedDesc:
                        users = users.OrderByDescending(s => s.IsFullBlocked);
                        break;
                    case SortState.IsFamilyDependentAsc:
                        users = users.OrderBy(s => s.IsDependent);
                        break;
                    case SortState.IsFamilyDependentDesc:
                        users = users.OrderByDescending(s => s.IsDependent);
                        break;
                    case SortState.IsOnlineAsc:
                        users = users.OrderBy(s => s.IsOnline);
                        break;
                    case SortState.IsOnlineDesc:
                        users = users.OrderByDescending(s => s.IsOnline);
                        break;
                    case SortState.GenderAsc:
                        users = users.OrderBy(s => s.Gender);
                        break;
                    case SortState.GenderDesc:
                        users = users.OrderByDescending(s => s.Gender);
                        break;

                    default:
                        users = users.OrderBy(s => s.Email);
                        break;
                }

                var count = await users.CountAsync();
                var items = await users.Skip((page - 1) * pageSize).Take(pageSize)
                           .Select(user => _mapper.Map<UserDto>(user)).ToListAsync();

                string pageRoute = @"/adminpanel/userindex?page=";
                string processOptions = $"&name={name}&roleid={roleId}&sortorder={sortOrder}";

                UserIndexViewModel viewModel = new (
                    items, processOptions,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterViewModel(roles, roleId, name),
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
        public async Task<IActionResult> UserDetails(string? id)
        {
            var baseModel = await _modelBuilder.BuildByIdAdmin(id);
            var model = _mapper.Map<AdminUserEditModel>(baseModel);

            var allroles = await _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToListAsync();

            if (model != null && allroles!=null && model.RoleNames!=null)
            {
                if (model.IsFullBlocked == true)
                {
                    var item = model?.BlockState?.First(x => x.IntId == 2);
                    item!.IsSelected = true;

                }
                else if (model.IsFullBlocked == false)
                {
                    var item = model?.BlockState?.First(x => x.IntId == 1);
                    item!.IsSelected = true;
                }
                else
                {
                    var item = model?.BlockState?.First(x => x.IntId == 0);
                    item!.IsSelected = true;
                }

                model!.AllRoles = allroles;

                foreach (var item in model.AllRoles)
                {
                    if (model.RoleNames.Any(x => x.Equals(item.Name)))
                    {
                        item.IsSelected = true;
                    }
                }
               
                return View(model);
            }
                
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UserDetails(AdminUserEditModel model)
        {
            if (model == null || model.Id==null)
                return BadRequest();
            int addresult = 0;
            int subtract = 0;
            int changeRes = 0;
           try
           {
                var baseModel = await _modelBuilder.BuildByIdAdmin(model.Id.ToString());
                var modelFull = _mapper.Map<AdminUserEditModel>(baseModel);
                var allroles = await _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToListAsync();
                modelFull.AllRoles = allroles;

                if (model.BlockStateIds != null)
              {
                foreach (var item in model.BlockStateIds)
                {
                    if (item == 1)
                            modelFull.IsFullBlocked = false;
                    if (item == 2)
                            modelFull.IsFullBlocked = true;
                }
                    changeRes=await _userService.ChangeUserFullBlockById((Guid)model.Id, modelFull?.IsFullBlocked);
              }
             if (model?.RoleIds != null && modelFull != null)
             {
                if (model.Id != null)
                {
                    foreach (var roleId in model.RoleIds)
                    {
                        var role = modelFull?.AllRoles?.FirstOrDefault(role => role.Id.Equals(roleId));

                        if (role != null)
                        {
                            role.IsSelected = true;
                            addresult += await _roleService.AddRoleByNameToUser((Guid)(model?.Id!), role?.Name!);
                        }
                    }
                    foreach (var role in modelFull.AllRoles!)
                    {
                        if (model!.RoleIds.All(r => r != role.Id))
                        {
                            role.IsSelected = false;
                            subtract += await _roleService.RemoveRoleByNameFromUser((Guid)(model?.Id!), role?.Name!);
                        }
                    }

                    model.SystemInfo = $"<b>Roles:<br/>{addresult} were added<br/>{subtract} were deleted</b>";
                        if (changeRes > 0)
                        {
                            model.SystemInfo += $"<b>Full block status was changed</b>";
                        }
                        else
                        {
                            model.SystemInfo += $"<br/><b>Full block status was not changed</b>";
                        }

                    return View(modelFull);
                }
             }
             else
             {
                model!.SystemInfo = $"<b>You have not chosen any role</b>";
                return View(modelFull);
            }

            model.SystemInfo = "<b>Something went wrong (</b>";
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
