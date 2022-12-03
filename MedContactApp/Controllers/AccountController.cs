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
using MedContactApp.Helpers;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customerDto = _mapper.Map<UserDto>(model);
                    if (customerDto != null && !string.IsNullOrEmpty(model.Email))
                    {
                        var result = await _userService.CreateUserWithRoleAsync(customerDto, "Customer");
                        if (result > 0)
                        {
                            await Authenticate(model.Email,customerDto.Id );
                            return RedirectToAction("Welcome", "Account");
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

        public IActionResult Welcome()
        {
            return View();
        }



        [HttpGet]
        public IActionResult RestrictedLogin()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model.Email != null && model.Password != null)
            {
                //var isPasswordCorrect = await _userService.CheckUserPassword(model.Email, model.Password);
                var res = await _userService.GetIdByEmailUserPassword(model.Email, model.Password);
                if (res.IntResult==1 && res.GuidResult!=null)
                {
                    //model.SystemInfo = "Your password has been reset";
                    //return View(model);
                    return RedirectToAction("PasswordRestore", "Account", new { id = res.GuidResult });
                }
                if (res.IntResult == 2 && res.GuidResult != null)             //(isPasswordCorrect)
                {
                    await Authenticate(model.Email, (Guid)res.GuidResult);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    model.SystemInfo = "Incorrect email or password";
                    return View(model);
                }
            }
           return View();
        }

        [HttpGet]
        public async Task<IActionResult> PasswordRestore(string id)
        {
            PasswordRestoreModel model = new();
            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid userId) && 
                (await _userService.GetUserByIdAsync(userId)!=null))
            {
                model.Id = userId;
                return View(model);
            }

            else
                return new BadRequestObjectResult("Something went wrong...");

        }

        [HttpPost]
        public async Task<IActionResult> PasswordRestore(PasswordRestoreModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SystemInfo = "Input correct data, please";
                return View(model);
            }
            if (model.Id == null)
                return new BadRequestObjectResult("User Id is null");
            var user = await _userService.GetUserByIdAsync((Guid)model.Id);
            if (user == null)
                return NotFound("User is not found");

            if (user.Username != null && user.Username.Equals(model.Codeword)==true &&
                user.BirthDate != null && user.BirthDate.Equals(model.BirthDate) == true &&
                user.Email != null && user.Email.Equals(model.Email)==true && model.Password != null)
            {
                var res = await _userService.ChangeUserPasswordAsync(user.Id, model.Password);
                if (res>0)
                {
                    await Authenticate(user.Email, user.Id);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    model.SystemInfo = "Password has not been changed";
                    return View(model);
                }
            }
            else
            {
                model.SystemInfo = "Input correct data";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var UserIdClaim = HttpContext.User.FindFirst("MUId");
                var userId = UserIdClaim!.Value;
                var result = Guid.TryParse(userId, out Guid guid_id);
                if (result)
                    await _userService.ChangeUserStatusById(guid_id, 1);

                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        
        private async Task Authenticate (string email, Guid userId)
        {
            string isfullBlocked="false";
            try
            {
                var dto = await _userService.GetUserByIdAsync(userId);
                var roleList = await _roleService.GetRoleListByUserIdAsync(dto.Id);

                if (dto.Email != null && roleList != null) // && dto.Username != null
                {
                    if (dto.IsFullBlocked == true)
                        isfullBlocked = "true";


                    string id = dto.Id.ToString();
                    var claims = new List<Claim>()
            {
                //new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Username),
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim("MUId",id),
                new Claim("UId",id),
                new Claim("FullBlocked", isfullBlocked)
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
                    await _userService.ChangeUserStatusById(dto.Id, 0);
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");    
            }

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
                try 
                {

                    if (sUserId != null && sMainUserId != null)
                    {
                        if (sUserId.Value.Equals(sMainUserId.Value))
                        {
                            if (Guid.TryParse(sUserId!.Value, out Guid userId))
                            {
                                var user = await _userService.GetUserByIdAsync(userId);
                                var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                                if (user != null && roles != null)
                                {
                                    string fName = user.Name + " " + user.Surname;
                                    UserDataModel model = new()
                                    {
                                        ActiveEmail = user.Email,
                                        ActiveFullName = fName,
                                        MainEmail = user.Email,
                                        MainFullName = fName,
                                        RoleNames = roles
                                    };
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
                }

                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                }
            }

            return View();
        }

    }
}
