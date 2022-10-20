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

namespace MedContactApp.Controllers
{
    public class UserPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFamilyService _familyService;
        private readonly IMapper _mapper;
        //private int _pageSize = 7;
        private readonly IConfiguration _configuration;
        private readonly EmailChecker _emailChecker;
        private readonly IRoleService _roleService;

        public UserPanelController(IUserService userService, IConfiguration configuration,
            IMapper mapper, IRoleService roleService,
            EmailChecker emailChecker, IFamilyService familyService)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _roleService = roleService;
            _emailChecker = emailChecker;
            _familyService = familyService;
        }
        public IActionResult Welcome()
        {
            return View();
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
            Guid MUid = Guid.Parse(mainUserId!.Value);
            var mainUserDto = await _userService.GetUserByIdAsync(MUid);

            RelativeModel model = new() { Email = mainUserDto.Email, Surname = mainUserDto.Surname, 
                          Address = mainUserDto.Address, PhoneNumber = mainUserDto.PhoneNumber };

            return View(model);
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

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            return Json(await _emailChecker.CheckEmail(email.ToLower()));
        }
    }
}
