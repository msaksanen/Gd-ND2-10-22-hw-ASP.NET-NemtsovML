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
                    model.SystemInfo = "Incorrect username or password.";
                    return View(model);

                }
            }
           return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");


        }

        [HttpGet]
        public async Task<IActionResult> AccSettings()
        {
            var mainUserId = User.FindFirst("MUId");
            if (Guid.TryParse(mainUserId!.Value, out Guid MUid))
            {
                var mainUserDto = await _userService.GetUserByIdAsync(MUid);
                return View(mainUserDto);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AccSettingsEdit(string? id)
        {
            var userDto = await GetUserDtoByIdAsync(id);
            if (userDto != null)
                return View(userDto);

            ModelState.AddModelError("CustomError", $"Doctor with id {id} is not found.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AccSettingsEdit(CustomerModel model)
        {
            try
            {
                if (model != null)
                {
                    var dto = _mapper.Map<UserDto>(model);
                    var sourceDto = await _userService.GetUserByIdAsync(dto.Id);
                    dto.RegistrationDate = sourceDto.RegistrationDate;
                    PatchMaker<UserDto> patchMaker = new();
                    var patchList = patchMaker.Make(dto, sourceDto);
                    await _userService.PatchAsync(dto.Id, patchList);
                    return RedirectToAction("AccSettings", "Account");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public  IActionResult ChangePassword(string id)
        {
            bool result=Guid.TryParse(id, out Guid userId);
            if (result)
            {
                ChangePasswordModel model = new() {Id=userId };
                return View(model);
            }  

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword (ChangePasswordModel model)
        {
            if (model !=null && model.Id != null && model.Password != null && model.OldPassword!=null)
            {
                try
                {
                    if (await _userService.CheckUserPassword((Guid)model.Id, model.OldPassword))
                    {
                        var result = await _userService.ChangeUserPasswordAsync((Guid)model.Id, model.Password);
                        if (result > 0)
                        {
                            model.SystemInfo = "The password has been changed successfully.";
                            return View(model);
                        }
                    }
                    else
                    {
                        model.OldPwdInfo = "You have entered incorrect password";
                        return View(model);
                    }
                    
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
            }

            ChangePasswordModel model1 = new() { SystemInfo = "Something went wrong (." };
            return View(model1);
        }

        private async Task<UserDto?> GetUserDtoByIdAsync(string? id)
        {
            var result = Guid.TryParse(id, out Guid guid_id);

            if (result)
            {
                var usr = await _userService.GetUserByIdAsync(guid_id);
                var usrDto = _mapper.Map<UserDto>(usr);
                return usrDto;
            }
            return null;
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
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim("MUId",id),
                new Claim("UId",id)
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

                if (sUserId != null && sMainUserId != null)
                {
                    if (sUserId.Value.Equals(sMainUserId.Value))
                    {
                        if (Guid.TryParse(sUserId!.Value, out Guid userId))
                        {
                            var user = await _userService.GetUserByIdAsync(userId);
                            if (user != null)
                            {
                                string fName = user.Name + " " + user.Surname;
                                UserDataModel model = new()
                                {
                                    ActiveEmail = user.Email,
                                    ActiveFullName = fName,
                                    MainEmail = user.Email,
                                    MainFullName = fName
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

            return View();
        }

    }
}
