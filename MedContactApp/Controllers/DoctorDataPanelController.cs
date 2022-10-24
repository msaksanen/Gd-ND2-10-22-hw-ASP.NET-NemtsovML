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
    public class DoctorDataPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IDoctorDataService _doctorDataService;
        private readonly ISpecialityService _specialityService;

        public DoctorDataPanelController(IUserService userService,
             IMapper mapper, IRoleService roleService, IDoctorDataService doctorDataService, 
             ISpecialityService specialityService)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _doctorDataService = doctorDataService;  
            _specialityService = specialityService;
        }


        [HttpGet]
        public async Task<IActionResult> EditDoctorData()
        {
            EditDoctorDataModel model = new();
           //model.Specialities = await _specialityService.GetSpecialitiesListAsync();
            var UserIdClaim = User.FindFirst("MUId");
            var userId = UserIdClaim!.Value;
            if (Guid.TryParse(userId, out Guid Uid))
            {
                model = await DoctorDataModelBuildAsync(model, Uid);
            }

            return View(model);
        }

        private async Task<EditDoctorDataModel> DoctorDataModelBuildAsync(EditDoctorDataModel model, Guid Uid)
        {
            model.Specialities = await _specialityService.GetSpecialitiesListAsync();
            if (model.Specialities != null)
            {
                var doctorData = await _doctorDataService.GetDoctorDataByUserId(Uid);
                foreach (var item in doctorData)
                {
                    var spec = model.Specialities.FirstOrDefault(sp => sp.Id.Equals(item.SpecialityId));
                    if (spec != null) spec.IsSelected = true;
                }
            }
            return model;
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctorData(EditDoctorDataModel model)
        {
            var UserIdClaim = User.FindFirst("MUId");
            var userId = UserIdClaim!.Value;
            var roleId = await _roleService.GetRoleIdByNameAsync("Doctor");
            //model.Specialities = await _specialityService.GetSpecialitiesListAsync();
            int addresult = 0;
            int subtract = 0;

            if (Guid.TryParse(userId, out Guid Uid) && roleId!=null)
            {
                model = await DoctorDataModelBuildAsync(model, Uid);

                if (model.Password != null && await _userService.CheckUserPassword(Uid, model.Password))
                {
                    if (model.SpecialityIds!=null)
                    {
                        var doctorData = await _doctorDataService.GetDoctorDataByUserId(Uid);

                        foreach (var spec in model.SpecialityIds)
                        {
                            if (doctorData.All(ddt => ddt.SpecialityId != spec))
                            {
                                var specModel = model?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(spec));
                                if (specModel != null) specModel.IsSelected = true;

                                DoctorDataDto doctorDataDto = new()
                                {
                                    Id = Guid.NewGuid(),
                                    IsBlocked = true,
                                    UserId = Uid,
                                    SpecialityId = spec,
                                    RoleId = roleId
                                };
                                addresult += await _doctorDataService.CreateDoctorDataAsync(doctorDataDto);
                            }  
                        }

                        foreach (var dd in doctorData) 
                        { 
                            if (model!.SpecialityIds.All(spec =>spec!=dd.SpecialityId ))
                            {
                                var specModel = model?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(dd.SpecialityId));
                                if (specModel != null) specModel.IsSelected = false;

                                subtract += await _doctorDataService.DeleteDoctorDataAsync(dd);
                            }
                        }

                        model.SystemInfo = $"Specialities: {addresult} were added; {subtract} were deleted";
                        return View(model);
                    }
                    else 
                    {
                        model.SystemInfo = "You have not chosen any speciality";
                        return View(model);
                    }
                }
                model.SystemInfo = "You have entered incorrect password";
                return View(model);
            }
            model.SystemInfo = "Something went wrong (.";
            return View (model);
        }

        //[HttpGet]
        //public async Task<IActionResult> AccSettings(string? id)
        //{
        //    string userId;
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        var UserIdClaim = User.FindFirst("MUId");
        //        userId = UserIdClaim!.Value;
        //    }
        //    else
        //    {
        //        userId = id;
        //    }
        //    if (Guid.TryParse(userId, out Guid Uid))
        //    {
        //        var UserDto = await _userService.GetUserByIdAsync(Uid);
        //        return View(UserDto);
        //    }
        //    return View();
        //}



        //[HttpGet]
        //public async Task<IActionResult> AccSettingsEdit(string? id)
        //{
        //    var userDto = await GetUserDtoByIdAsync(id);
        //    if (userDto != null)
        //        return View(userDto);

        //    ModelState.AddModelError("CustomError", $"Doctor with id {id} is not found.");
        //    return RedirectToAction("Index", "Home");
        //}

        //[HttpPost]
        //public async Task<IActionResult> AccSettingsEdit(CustomerModel model)
        //{
        //    try
        //    {
        //        if (model != null)
        //        {
        //            var dto = _mapper.Map<UserDto>(model);
        //            var sourceDto = await _userService.GetUserByIdAsync(dto.Id);
        //            dto.RegistrationDate = sourceDto.RegistrationDate;
        //            PatchMaker<UserDto> patchMaker = new();
        //            var patchList = patchMaker.Make(dto, sourceDto);
        //            await _userService.PatchAsync(dto.Id, patchList);
        //            return RedirectToAction("AccSettings", "UserPanel");
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, ex.Message);
        //        return StatusCode(500);
        //    }
        //}

        //[HttpGet]
        //public IActionResult ChangePassword(string id)
        //{
        //    bool result = Guid.TryParse(id, out Guid userId);
        //    if (result)
        //    {
        //        ChangePasswordModel model = new() { Id = userId };
        //        return View(model);
        //    }

        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        //{
        //    if (model != null && model.Id != null && model.Password != null && model.OldPassword != null)
        //    {
        //        try
        //        {
        //            if (await _userService.CheckUserPassword((Guid)model.Id, model.OldPassword))
        //            {
        //                var result = await _userService.ChangeUserPasswordAsync((Guid)model.Id, model.Password);
        //                if (result > 0)
        //                {
        //                    model.SystemInfo = "The password has been changed successfully.";
        //                    return View(model);
        //                }
        //            }
        //            else
        //            {
        //                model.OldPwdInfo = "You have entered incorrect password";
        //                return View(model);
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
        //            return BadRequest();
        //        }
        //    }

        //    ChangePasswordModel model1 = new() { SystemInfo = "Something went wrong (." };
        //    return View(model1);
        //}

        //private async Task<UserDto?> GetUserDtoByIdAsync(string? id)
        //{
        //    var result = Guid.TryParse(id, out Guid guid_id);

        //    if (result)
        //    {
        //        var usr = await _userService.GetUserByIdAsync(guid_id);
        //        var usrDto = _mapper.Map<UserDto>(usr);
        //        return usrDto;
        //    }
        //    return null;
        //}

    }
}
