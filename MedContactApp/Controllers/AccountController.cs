﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var customerRole = await _roleService.GetRoleByNameAsync("Customer");
                    var customerDto = _mapper.Map<UserDto>(model);
                    if (customerDto != null && !string.IsNullOrEmpty(model.Email))
                    {
                        var result = await _userService.CreateUserWithRoleAsync(customerDto, "Customer");
                        if (result > 0)
                        {
                            await Authenticate(model.Email,customerDto.Id );
                            return RedirectToAction("Welcome", "UserPanel");
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

        [HttpGet]
        public async Task<IActionResult> UserLoginPreview()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {

                var sUserId = User.FindFirst("UId");
                var sMainUserId = User.FindFirst("MUId");

                if (sUserId == null)
                {
                    return BadRequest();
                }

                if (sMainUserId == null)
                {
                    return BadRequest();
                }

                if (sUserId!= null && sMainUserId != null)
                {
                    if (sUserId.Value.Equals(sMainUserId.Value)) 
                    {
                        if (Guid.TryParse(sUserId!.Value, out Guid userId)) 
                        {
                            var user = await _userService.GetUserByIdAsync(userId);
                            if (user != null)
                            {
                                string fName = user.Name + " " + user.Surname;
                                UserDataModel model = new() { ActiveEmail = user.Email, ActiveFullName = fName,
                                MainEmail = user.Email, MainFullName = fName};
                                return View(model);
                            }                           
                        }      
                    }
                    else
                    {
                        UserDataModel model = new UserDataModel();
                       
                        if (Guid.TryParse(sUserId!.Value, out Guid userId))
                        {
                            var user = await _userService.GetUserByIdAsync(userId);
                            if (user != null)
                            {
                                string fName = user.Name + " " + user.Surname;
                                model.ActiveFullName = fName;
                                model.ActiveEmail = user.Email;
                            }
                        }
                        if (Guid.TryParse(sMainUserId!.Value, out Guid mainUserId))
                        {
                            var mainUser = await _userService.GetUserByIdAsync(mainUserId);
                            if (mainUser != null)
                            {
                                string fName = mainUser.Name + " " + mainUser.Surname;
                                model.MainFullName = fName;
                                model.MainEmail = mainUser.Email;
                            }
                        }
                        return View(model);
                    }
                }

               
                //var user = await _userService.GetUserByEmailAsync(userEmail);
                //var user = _mapper.Map<UserDataModel>(await _userService.GetUserByEmailAsync(userEmail.Value));
                //var roleList = await _roleService.GetRoleListByUserIdAsync(user.Id);           
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email);
            var userId = User.FindFirst("UId");
            var result = Guid.TryParse(userId!.Value, out Guid usrId);
            if (string.IsNullOrEmpty(userEmail?.Value) && !result)
            {
                return BadRequest();
            }
           
                var user = _mapper.Map<UserDataModel>(await _userService.GetUserByIdAsync(usrId));
                return Ok(user);
        }
               

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
           if (model.Email != null && model.Password != null)
           {
                //var isPasswordCorrect = await _userService.CheckUserPassword(model.Email, model.Password);
                var id = await _userService.GetIdByEmailUserPassword(model.Email, model.Password);
            if (id != null)             //(isPasswordCorrect)
            {
                await Authenticate(model.Email, (Guid)id);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
           }
           return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");


        }

        private async Task Authenticate (string email, Guid userId)
        {
            var dto = await _userService.GetUserByIdAsync(userId);
            var roleList = await _roleService.GetRoleListByUserIdAsync(dto.Id);

            if (dto.Email != null && dto.Username!=null  && roleList!=null)
            {
                string id = dto.Id.ToString();
                var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Username),
                //new Claim(ClaimTypes.Name, dto.Name!),
                //new Claim(ClaimTypes.Surname, dto.Surname!),
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim("MUId",id),
                new Claim("UId",id)
            };

                //if (dto.FamilyId != null)
                //{
                //    string fId = dto.FamilyId.GetValueOrDefault().ToString();
                //    claims.Add(new Claim("FId", fId));
                //}
                //if (dto.Name != null) claims.Add(new Claim(ClaimTypes.Name, dto.Name));
                //if (dto.Surname != null) claims.Add(new Claim(ClaimTypes.Surname, dto.Surname));

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
