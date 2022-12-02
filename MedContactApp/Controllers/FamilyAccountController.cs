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
using MedContactApp.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MedContactApp.Controllers
{
    public class FamilyAccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFamilyService _familyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;
        private readonly ModelUserBuilder _modelBuilder;

        public FamilyAccountController(IUserService userService, IConfiguration configuration,
            IMapper mapper, IRoleService roleService, ModelUserBuilder modelBuilder,
        IFamilyService familyService)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _roleService = roleService;
            _familyService = familyService;
            _modelBuilder = modelBuilder;
        }
       

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Family()
        {
            var mainUserId = User.FindFirst("MUId");
            try
            {
                var res = Guid.TryParse(mainUserId!.Value, out Guid MUid);
                if (!res)
                    return new BadRequestObjectResult("Main User is not found" );
                var customers = await _familyService.GetRelativesAsync(MUid);
                return View(customers);    
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> AddRelative()
        {
            var mainUserId = User.FindFirst("MUId");
            if (Guid.TryParse(mainUserId?.Value, out Guid MUid))
            {
                var mainUserDto = await _userService.GetUserByIdAsync(MUid);
                HttpContext.Session.SetInt32("isDependent", 1);

                RelativeModel model = new()
                {
                    Email = mainUserDto.Email,
                    Surname = mainUserDto.Surname,
                    Address = mainUserDto.Address,
                    PhoneNumber = mainUserDto.PhoneNumber
                };
                return View(model);
            }
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> AddRelative(RelativeModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.IsDependent = true;
                    var relativeDto = _mapper.Map<UserDto>(model);
                    if (relativeDto != null)
                    {
                        int result = 0;
                        var mainUserId = User.FindFirst("MUId");
                        Guid MUid = Guid.Parse(mainUserId!.Value);
                        var mainUserDto = await _userService.GetUserByIdAsync(MUid);
                        if (mainUserDto!= null)
                        {
                            if (mainUserDto.FamilyId != null)
                            {
                                relativeDto.FamilyId = mainUserDto.FamilyId;
                                result += await _userService.CreateUserWithRoleAsync(relativeDto, "Customer");
                                if (result > 0)
                                {
                                    if (HttpContext.Session.Keys.Contains("isDependent"))
                                        HttpContext.Session.SetInt32("isDependent", 0);
                                    return RedirectToAction("Family", "FamilyAccount");
                                }       
                            }
                            else
                            {
                                FamilyDto newfamilyDto = new FamilyDto() { Id = Guid.NewGuid(), MainUserId = mainUserDto.Id };
                                mainUserDto.FamilyId = newfamilyDto.Id;
                                relativeDto.FamilyId = newfamilyDto.Id;
                                result = await _familyService.CreateNewFamilyForMainUser(mainUserDto, newfamilyDto);
                                result += await _userService.CreateUserWithRoleAsync(relativeDto, "Customer");    
                            }
                        }
                        if (result > 3)
                        {
                            if (HttpContext.Session.Keys.Contains("isDependent"))
                                HttpContext.Session.SetInt32("isDependent", 0);
                            return RedirectToAction("Family", "FamilyAccount");
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
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> SetActiveRelative(string? id)
        {
            var result= Guid.TryParse(id, out Guid relativeId);
            if (result)
            {
                var relativeDto= await _userService.GetUserByIdAsync(relativeId);
                if(relativeDto != null)
                {
                   await ChangeClaims(relativeDto);
                }
                return RedirectToAction("Family", "FamilyAccount");
            }

                var mainUserId = User.FindFirst("MUId");
                Guid MUid = Guid.Parse(mainUserId!.Value);
                var mainUserDto = await _userService.GetUserByIdAsync(MUid);
                await ChangeClaims(mainUserDto);

            return Redirect(Request.Headers["Referer"].ToString());

        }


        private async Task ChangeClaims(UserDto relativeDto)
        {
            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userId = claimsIdentity.FindFirst("UId");
                if (claimsIdentity.TryRemoveClaim(userId))
                    claimsIdentity.AddClaim(new Claim("UId", relativeDto.Id.ToString()));

                //var username = claimsIdentity.FindFirst(ClaimsIdentity.DefaultNameClaimType);
                //if (claimsIdentity.TryRemoveClaim(username) && relativeDto.Username !=null)
                //    claimsIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, relativeDto.Username));
                 
                var name = claimsIdentity.FindFirst(ClaimTypes.Name);
                if (claimsIdentity.TryRemoveClaim(name) && relativeDto.Name != null)
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, relativeDto.Name));

                var surname = claimsIdentity.FindFirst(ClaimTypes.Surname);
                if (claimsIdentity.TryRemoveClaim(surname) && relativeDto.Surname != null)
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, relativeDto.Surname));

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
     
            }
            
        }
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> RemoveAccount(string? id)
        {
            var model = await _modelBuilder.BuildById(HttpContext, id);
            if (model != null)
            {
                var chModel = _mapper.Map<ChangePasswordModel>(model);
                chModel.Username = String.Empty;
                return View(chModel);
            }

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> RemoveAccount(ChangePasswordModel model)
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated) &&
                 model != null && model.Id != null && model.Password != null && model.Username != null)
            {
                try
                {
                    var sMainUserId = User.FindFirst("MUId");
                    //var sUserId = User.FindFirst("UId");
                    if (sMainUserId == null) // || sUserId==null
                    {
                        return new BadRequestObjectResult("User is not found. Something went wrong");
                    }
                    var mRes = Guid.TryParse(sMainUserId.Value, out Guid MUId);
                    //var sRes = Guid.TryParse(sMainUserId.Value, out Guid UId);
                    if (!mRes)
                    {
                        return new BadRequestObjectResult("User is not found. Something went wrong");
                    }
                    //if (!sRes)
                    //{
                    //    return new BadRequestObjectResult("User is not found. Something went wrong");
                    //}
                    var res = await _userService.CheckUserByUsernamePassword(MUId, model.Password, model.Username);
                    if (res == 0)
                    {
                        model.SystemInfo = "User is not found. Something went wrong";
                        return View(model);
                    }
                    if (res == 1)
                    {
                        model.SystemInfo = "You have entered wrong password or code name";
                        return View(model);
                    }
                    else
                    {
                        var relatives = await _familyService.GetRelativesAsync((Guid)model.Id);
                        if (relatives != null && relatives.Count > 1)
                        {
                            model.SystemInfo = "You should delete all accounts of your relatives at first";
                            return View(model);
                        }
                        else
                        {
                            var result = await _userService.RemoveUserById((Guid)model.Id);
                            if (result > 0)
                            { 
                                if (MUId.Equals(model.Id))
                                {
                                    await HttpContext.SignOutAsync();
                                    return RedirectToAction("Deleted", "FamilyAccount");
                                }
                                else
                                {
                                    var usr = await _userService.GetUserByIdAsync(MUId);
                                    await ChangeClaims(usr);
                                    return RedirectToAction("Family", "FamilyAccount");
                                }
                            }
                            else
                            {
                                model.SystemInfo = "Your account has not been removed<br/>Something went wrong";
                                return View(model);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
            }
            ChangePasswordModel model1 = new() { SystemInfo = "Something went wrong" };
            return View(model1);
        }

        public IActionResult Deleted()
        {
            return View();
        }


    }
}
