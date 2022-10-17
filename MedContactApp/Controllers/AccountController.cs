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

namespace MedContactApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;

        public AccountController(IUserService userService,
            IMapper mapper, IRoleService roleService)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
        }

        [HttpGet]
        public IActionResult RegCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegCustomer(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var customerRole = await _roleService.GetRoleByNameAsync("Customer");
                    var customerDto = _mapper.Map<UserDto>(model);
                    if (customerDto != null) // && customerRole != null)
                    {
                        var result = await _userService.CreateUserWithRoleAsync(customerDto, "Customer");
                        if (result > 0)
                        {
                            //await Authenticate(model.Email);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegDoctor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegDoctor(DoctorModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var customerRole = await _roleService.GetRoleByNameAsync("Doctor");
                    var customerDto = _mapper.Map<UserDto>(model);
                    if (customerDto != null) // && customerRole != null)
                    {
                        var result = await _userService.CreateUserWithRoleAsync(customerDto, "Doctor");
                        if (result > 0)
                        {
                            //await Authenticate(model.Email);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
            }
            return View(model);
        }

        private async Task Authenticate (string email)
        {
            var dto= await _userService.GetUserByEmailAsync(email);
            var roleList = await _roleService.GetRoleListByUserIdAsync(dto.Id);

            if (dto.Email != null && roleList!=null)
            {
                var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Email)
            };

                foreach (var role in roleList)
                    if (role.Name != null)
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));

                var identity = new ClaimsIdentity(claims,
                    "ApplicationCookie",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
           
        }

    }
}
