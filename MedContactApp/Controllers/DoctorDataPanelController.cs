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
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MedContactApp.Controllers
{
    public class DoctorDataPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ModelUserBuilder _modelUserBuilder;
        private readonly IRoleService _roleService;
        private readonly IDoctorDataService _doctorDataService;
        private readonly ISpecialityService _specialityService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IFileDataService _fileDataService;
        private readonly FileValidation _fileValidation;

        public DoctorDataPanelController(IUserService userService,
             IMapper mapper, IRoleService roleService, IDoctorDataService doctorDataService,
             ISpecialityService specialityService, IWebHostEnvironment appEnvironment,
             IFileDataService fileDataService, ModelUserBuilder modelUserBuilder, FileValidation fileValidation)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _doctorDataService = doctorDataService;
            _specialityService = specialityService;
            _appEnvironment = appEnvironment;
            _fileDataService = fileDataService;
            _modelUserBuilder = modelUserBuilder;
            _fileValidation = fileValidation;
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
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
            StringBuilder sb = new();
            string init = "<b>Marked for deletion specialities:<br/>";
            sb = sb.Append(init);
            model.UserId = Uid;
            if (model.Specialities != null)
            {
                var doctorData = await _doctorDataService.GetDoctorDataListByUserId(Uid);
                foreach (var item in doctorData)
                {
                    var spec = model.Specialities.FirstOrDefault(sp => sp.Id.Equals(item.SpecialityId));
                    if (spec != null && item.ForDeletion != true) spec.IsSelected = true;
                    if (spec != null && item.ForDeletion == true) sb.Append(spec.Name + "<br/>");
                }
            }
            if (sb.Length > init.Length)
            {
                sb.Append("</b>");
                model.SystemInfo = sb.ToString();
            }
            return model;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> EditDoctorData(EditDoctorDataModel model)
        {
          try
          {
            var UserIdClaim = User.FindFirst("MUId");
            var userId = UserIdClaim?.Value;
            var roleId = await _roleService.GetRoleIdByNameAsync("Doctor");
            //model.Specialities = await _specialityService.GetSpecialitiesListAsync();
            int addresult = 0;
            int subtract = 0;

            if (Guid.TryParse(userId, out Guid Uid) && roleId != null)
            {
                model = await DoctorDataModelBuildAsync(model, Uid);
                var doctorData = await _doctorDataService.GetDoctorDataListByUserId(Uid);

                if (model.Password != null && await _userService.CheckUserPassword(Uid, model.Password))
                {
                        if (model.SpecialityIds != null)
                        {
                            foreach (var spec in model.SpecialityIds)
                            {
                                if (doctorData.All(ddt => ddt.SpecialityId != spec) || 
                                    doctorData.Any(ddt => ddt.SpecialityId == spec && ddt.ForDeletion==true))
                                {
                                    var specModel = model?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(spec));
                                    if (specModel != null) specModel.IsSelected = true;

                                    DoctorDataDto doctorDataDto = new()
                                    {
                                        Id = Guid.NewGuid(),
                                        IsBlocked = true,
                                        UserId = Uid,
                                        SpecialityId = spec,
                                        RoleId = roleId,
                                        SpecNameReserved = specModel?.Name
                                    };
                                    addresult += await _doctorDataService.CreateDoctorDataAsync(doctorDataDto);
                                }
                            }
                        }   
                        foreach (var dd in doctorData)
                        {
                            if (model!.SpecialityIds==null || model!.SpecialityIds.All(spec => spec != dd.SpecialityId))
                            {
                                var specModel = model?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(dd.SpecialityId));
                                if (specModel != null && dd.ForDeletion != true)
                                {
                                    specModel.IsSelected = false;
                                    subtract += await _doctorDataService.MarkForDeleteDoctorDataAsync(dd);
                                }
                            }
                        }

                        model.SystemInfo = $"<b>Specialities:<br/>{addresult} was/were added<br/>{subtract} was/were marked for deletion</b>";
                        return View(model);
                 
                }
                model.SystemInfo = "You have entered incorrect password";
                return View(model);
            }
            model.SystemInfo = "<b>Something went wrong (</b>";
            return View(model);
            }
          catch (Exception e)
          {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
          }
      }

        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task <IActionResult> RegDoctorData()
        {
            var model = await _modelUserBuilder.BuildById(HttpContext);
            if (model != null)
            {
                var regModel = _mapper.Map<RegDoctorDataModel>(model);

                return View(regModel);
            }

           //return NotFound();
            return new BadRequestObjectResult("Model is null");

        }

        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> RegDoctorData (RegDoctorDataModel model)
        {
            int result = 0;
            if (model.Uploads == null || model.Uploads.Count==0)
            {
                model.SystemInfo = "<b>You have not uploaded files. Try again, please</b>";
                return View(model);
            }
            var UserIdClaim = User.FindFirst("MUId");
            var userId = UserIdClaim?.Value;
            try
            {
                if (Guid.TryParse(userId, out Guid UId) && model.Password != null)
                {
                    if (await _userService.CheckUserPassword(UId, model.Password))
                    {
                        result = await _roleService.AddRoleByNameToUser(UId, "Applicant");
                        if (result > 0)
                            model.SystemInfo = "<b>You have been added to applicants<b><br/>";
                        else
                            model.SystemInfo = "<b>You have been added to applicants before<b><br/>";

                        StringBuilder sb = new StringBuilder();
                        List<FileDataDto> list = new();

                        foreach (var uploadedFile in model.Uploads)
                        {
                              var resSize = _fileValidation.SingleFileSizeValidation(uploadedFile);
                              var resExt = _fileValidation.SingleFileExtValidation(uploadedFile);
                              if (resSize.IntResult == 1 && resExt.IntResult == 1)
                              {
                                string ext = Path.GetExtension(uploadedFile.FileName);
                                string name = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + $"-{DateTime.Now:HH.mm-dd.MM.yyyy}" + ext;
                                string path = "/files/cv/" + name;
                                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                                {
                                    await uploadedFile.CopyToAsync(fileStream);
                                }
                                FileDataDto file = new FileDataDto
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = UId,
                                    Name = uploadedFile.FileName,
                                    Path = path,
                                    Category = "Applicant"
                                };
                                list.Add(file);
                              }
                              else
                              {
                                sb.Append($"<br/><b>{uploadedFile.FileName}<b/>: ");
                                if (resSize.IntResult == 0) sb.Append(resSize.Name);
                                if (resExt.IntResult == 0) sb.Append(resExt.Name);
                              }
                               
                        }
                            var fileResult = await _fileDataService.AddListToFileData(list);

                            if (fileResult > 1)
                                model.SystemInfo += $"<br/><b>{fileResult} files have been uploaded</b>";
                            else if (fileResult == 1)
                                model.SystemInfo += $"<br/><b> 1 file has been uploaded</b>";
                            else
                                model.SystemInfo += $"<br/><b> No files have been uploaded</b>";

                             model.SystemInfo += sb.ToString();

                             return View(model);
                    }
                        //var resSize = _fileValidation.FileSizeValidation(model.Uploads);
                        //var resExt = _fileValidation.FileExtValidation(model.Uploads);
                        //if (resSize.IntResult == 1 && resExt.IntResult == 1)
                        //{
                        //    List<FileDataDto> list = new();
                        //    if (result > 0)
                        //        model.SystemInfo = "<b>You have been added to applicants<br/>";
                        //    else
                        //        model.SystemInfo = "<b>You have been added to applicants before<br/>";

                        //    foreach (var uploadedFile in model.Uploads)
                        //    {
                        //        string ext = Path.GetExtension(uploadedFile.FileName);
                        //        string name = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + $"-{DateTime.Now:HH.mm-dd.MM.yyyy}" + ext;
                        //        string path = "/files/cv/" + name;
                        //        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        //        {
                        //            await uploadedFile.CopyToAsync(fileStream);
                        //        }
                        //        FileDataDto file = new FileDataDto
                        //        {
                        //            Id = Guid.NewGuid(),
                        //            UserId = UId,
                        //            Name = uploadedFile.FileName,
                        //            Path = path,
                        //            Category = "Applicant"
                        //        };
                        //        list.Add(file);
                        //    }
                        //    var fileResult = await _fileDataService.AddListToFileData(list);

                        //    if (fileResult > 1)
                        //        model.SystemInfo = model.SystemInfo + $"{fileResult} files have been uploaded</b>";
                        //    else if (fileResult == 1)
                        //        model.SystemInfo = model.SystemInfo + $"1 file has been uploaded</b>";
                        //    else
                        //        model.SystemInfo = model.SystemInfo + $"No files have been uploaded</b>";

                        //    return View(model);
                        //}
                        //else
                        //{
                        //    model.SystemInfo += $"<b> No files have been uploaded.</b>" + resSize.Name + resExt.Name;
                        //    return View(model);
                        //}
                    else
                    {
                        model.SystemInfo = "<b>You have entered incorrect password</b>";
                        return View(model);
                    }
                  
                }
                else
                {
                    model.SystemInfo = "<b>Something went wrong (</b>";
                    return View(model);
                }
   
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }
    }
}
