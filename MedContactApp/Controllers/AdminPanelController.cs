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

namespace MedContactApp.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private int _pageSize = 7;

        public AdminPanelController(IUserService userService,
            IMapper mapper, IRoleService roleService, IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
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
                    users = users.Where(p => p.Name!.Contains(name));
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
    }
}
