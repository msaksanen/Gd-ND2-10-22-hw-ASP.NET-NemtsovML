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

namespace MedContactApp.Controllers
{
    public class FamilyAccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFamilyService _familyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;

        public FamilyAccountController(IUserService userService, IConfiguration configuration,
            IMapper mapper, IRoleService roleService,
            IFamilyService familyService)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _roleService = roleService;
            _familyService = familyService;
        }
       

        [HttpGet]
        public async Task<IActionResult> Family()
        {
            var mainUserId = User.FindFirst("MUId");
            try
            {
                Guid MUid= Guid.Parse(mainUserId!.Value);
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
        public async Task<IActionResult> AddRelative()
        {
            var mainUserId = User.FindFirst("MUId");
            if (Guid.TryParse(mainUserId!.Value, out Guid MUid))
            {
                var mainUserDto = await _userService.GetUserByIdAsync(MUid);

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
                                    return RedirectToAction("Family", "UserPanel");
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
                            return RedirectToAction("Family", "UserPanel");
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
        public async Task<IActionResult> SetActiveRelative(string id)
        {
            var result= Guid.TryParse(id, out Guid relativeId);
            if (result)
            {
                var relativeDto= await _userService.GetUserByIdAsync(relativeId);
                if(relativeDto != null)
                {
                   await ChangeClaims(relativeDto);
                }
            }
            return RedirectToAction("Family", "FamilyAccount");

        }


        private async Task ChangeClaims(UserDto relativeDto)
        {
            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userId = claimsIdentity.FindFirst("UId");
                if (claimsIdentity.TryRemoveClaim(userId))
                    claimsIdentity.AddClaim(new Claim("UId", relativeDto.Id.ToString()));

                var username = claimsIdentity.FindFirst(ClaimsIdentity.DefaultNameClaimType);
                if (claimsIdentity.TryRemoveClaim(username) && relativeDto.Username !=null)
                    claimsIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, relativeDto.Username));
                 
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

    }
}
