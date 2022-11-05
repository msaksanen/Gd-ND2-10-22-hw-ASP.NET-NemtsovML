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
using System.Linq;

namespace MedContactApp.Controllers
{
    public class UserPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly ModelUserBuilder _modelBuilder;

        public UserPanelController(IUserService userService,
            IMapper mapper, IRoleService roleService, ModelUserBuilder modelBuilder)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _modelBuilder = modelBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> AccSettings(string? id)
        {
            var model = await _modelBuilder.BuildById(HttpContext, id);
            if (model!= null) 
                return View(model);

            return NotFound();
        }



        [HttpGet]
        public async Task<IActionResult> AccSettingsEdit(string? id)
        {  
            var model = await _modelBuilder.BuildById(HttpContext, id);
            if (model != null)
            {
                if (model.IsDependent == true)
                    HttpContext.Session.SetInt32("isDependent", 1);
                return View(model);
            }
            return NotFound();
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
                    if (HttpContext.Session.Keys.Contains("isDependent"))
                        HttpContext.Session.SetInt32("isDependent", 0);

                    return RedirectToAction("AccSettings", "UserPanel", new { id = model.Id });
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
        public async Task <IActionResult> ChangePassword(string? id)
        {
            var model = await _modelBuilder.BuildById(HttpContext, id);
            if (model != null)
            {
                var chModel =_mapper.Map<ChangePasswordModel>(model);
                return View(chModel);
            }
                
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (model != null && model.Id != null && model.Password != null && model.OldPassword != null)
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


    }
}
