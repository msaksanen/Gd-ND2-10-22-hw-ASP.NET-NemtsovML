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
using System.Data;
using System.Xml.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using MedContactDb.Migrations;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Linq.Expressions;
using MedContactApp.AdminPanelHelpers;

namespace MedContactApp.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly IFileDataService _fileDataService;
        private readonly IDoctorDataService _doctorDataService;
        private readonly ISpecialityService _specialityService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly AdminModelBuilder _adminModelBuilder;
        private readonly AdminSortFilter _adminSortFilter;
        private int _pageSize = 7;

        public AdminPanelController(IUserService userService,
            IMapper mapper, IRoleService roleService, IConfiguration configuration,
            ModelUserBuilder modelBuilder, IFileDataService fileDataService, IDoctorDataService doctorDataService,
            IWebHostEnvironment appEnvironment, ISpecialityService specialityService, AdminModelBuilder adminModelBuilder,
            AdminSortFilter adminSortFilter)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
            _modelBuilder = modelBuilder;
            _fileDataService = fileDataService;
            _doctorDataService = doctorDataService;
            _appEnvironment = appEnvironment;
            _specialityService = specialityService;
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
        public async Task<IActionResult> UserDetails(string? id, string? reflink = "")
        {

            AdminUserEditModel emptyModel = new();
            var model = await _adminModelBuilder.AdminUserModelBuildAsync(emptyModel, id);
            if (model == null)
                return NotFound();

            model.Reflink = reflink;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserDetails(AdminUserEditModel model)
        {
            if (model == null || model.Id == null)
                return BadRequest();
            int addresult = 0;
            int subtract = 0;
            int changeRes = 0;
            try
            {
                var modelFull = await _adminModelBuilder.AdminUserModelBuildAsync(model, String.Empty);
                if (modelFull == null) throw new Exception();

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