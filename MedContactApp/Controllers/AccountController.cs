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
                            await Authenticate(model.Email);
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

        [HttpGet]
        public async Task<IActionResult> UserLoginPreview()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {
                var userEmail = User.FindFirst(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail?.Value))
                {
                    return BadRequest();
                }
                //var user = await _userService.GetUserByEmailAsync(userEmail);
                var user = _mapper.Map<UserDataModel>(await _userService.GetUserByEmailAsync(userEmail.Value));
                //var roleList = await _roleService.GetRoleListByUserIdAsync(user.Id);
                return View(user);
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail?.Value))
            {
                return BadRequest();
            }

            var user = _mapper.Map<UserDataModel>(await _userService.GetUserByEmailAsync(userEmail.Value));
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
                var isPasswordCorrect = await _userService.CheckUserPassword(model.Email, model.Password);
            if (isPasswordCorrect)
            {
                await Authenticate(model.Email);
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

        private async Task Authenticate (string email)
        {
            var dto= await _userService.GetUserByEmailAsync(email);
            var roleList = await _roleService.GetRoleListByUserIdAsync(dto.Id);

            if (dto.Email != null && dto.Username!=null  && roleList!=null)
            {
                var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Username),
                new Claim(ClaimTypes.Email, dto.Email)
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
