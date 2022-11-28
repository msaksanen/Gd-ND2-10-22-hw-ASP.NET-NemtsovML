using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DoctorData.Commands;
using MedContactDataCQS.DoctorData.Queries;
using MedContactDataCQS.FileData.Commands;
using MedContactDataCQS.Roles.Commands;
using MedContactDataCQS.Roles.Queries;
using MedContactDataCQS.Tokens.Commands;
using MedContactDataCQS.Tokens.Queries;
using MedContactDataCQS.Users.Commands;
using MedContactDataCQS.Users.Queries;
using MedContactDb.Entities;
using MedContactWebApi.AdminPanelHelpers;
using MedContactWebApi.FilterSortPageHelpers;
using MedContactWebApi.Helpers;
using MedContactWebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// DoctorDataPanel Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DoctorDataPanelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly DataChecker _datachecker;
        private readonly IMediator _mediator;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly AdminSortFilter _adminSortFilter;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _appEnvironment;
        //private int _pageSize = 7;

        /// <summary>
        /// Doctor Controller Ctor
        /// </summary>
        public DoctorDataPanelController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, 
            DataChecker datachecker, IConfiguration configuration, ModelUserBuilder modelBuilder,
            AdminSortFilter adminSortFilter, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5; 
            _mediator = mediator;   
            _datachecker = datachecker;
            _modelBuilder = modelBuilder;
            _configuration = configuration;
            _adminSortFilter = adminSortFilter;
            _appEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// EditDoctorData (doctor's list of specialities)
        /// </summary>
        /// <returns>EditDoctorDataModel model</returns>
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

            return Ok(model);
        }

        /// <summary>
        /// EditDoctorData (doctor's list of specialities)
        /// Client should return selected specialities as Guid[] SpecialityIds
        /// </summary>
        /// <param name="response EditDoctorDataResponse [FromBody]"></param>
        /// <returns>EditDoctorDataModel model</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> EditDoctorData([FromBody]EditDoctorDataResponse response)
        {
            try
            {
                var UserIdClaim = User.FindFirst("MUId");
                var userId = UserIdClaim?.Value;
                var role = await _mediator.Send(new GetRoleByNameQuery() { RoleName = "Doctor" });
                //model.Specialities = await _specialityService.GetSpecialitiesListAsync();
                int addresult = 0;
                int subtract = 0;
                EditDoctorDataModel model = new() { Password = response.Password, SpecialityIds = response.SpecialityIds };
                if (Guid.TryParse(userId, out Guid Uid) && role?.Id != null)
                {
                    model = await DoctorDataModelBuildAsync(model, Uid);
                    var doctorData = await _mediator.Send(new GetDoctorDataListByUserIdQuery() { UserId = Uid });
                    var pwdHash = _md5.CreateMd5(model.Password);
                    var pwdCheckRes = await _mediator.Send(new CheckUserPwdHashByUserIdQuery()
                                                             { UserId = Uid, passwordHash = pwdHash });
                    if (model.Password != null && pwdCheckRes ==true && doctorData != null)
                    {
                        if (model.SpecialityIds != null)
                        {
                            foreach (var spec in model.SpecialityIds)
                            {
                                if (doctorData.All(ddt => ddt.SpecialityId != spec) ||
                                    doctorData.Any(ddt => ddt.SpecialityId == spec && ddt.ForDeletion == true))
                                {
                                    var specModel = model?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(spec));
                                    if (specModel != null) specModel.IsSelected = true;

                                    DoctorDataDto doctorDataDto = new()
                                    {
                                        Id = Guid.NewGuid(),
                                        IsBlocked = true,
                                        UserId = Uid,
                                        SpecialityId = spec,
                                        RoleId = role.Id,
                                        SpecNameReserved = specModel?.Name
                                    };
                                    addresult += await _mediator.Send(new CreateDoctorDataCommand() { DoctorDataDto = doctorDataDto });
                                }
                            }
                        }
                        foreach (var dd in doctorData)
                        {
                            if (model!.SpecialityIds == null || model!.SpecialityIds.All(spec => spec != dd.SpecialityId))
                            {
                                var specModel = model?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(dd.SpecialityId));
                                if (specModel != null && dd.ForDeletion != true)
                                {
                                    specModel.IsSelected = false;
                                    subtract += await _mediator.Send(new MarkForDeleteDoctorDataCommand() { DoctorDataDto = dd });
                                }
                            }
                        }

                        model.SystemInfo = $"<b>Specialities:<br/>{addresult} was/were added<br/>{subtract} was/were marked for deletion</b>";
                        return Ok(model);

                    }
                    model.SystemInfo = "You have entered incorrect password";
                    return BadRequest(model);
                }
                model.SystemInfo = "<b>Something went wrong (</b>";
                return BadRequest(model);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }


        /// <summary>
        /// RegDoctorData (doctor's list of specialities)
        /// Client should send files for application
        /// </summary>
        /// <param name="model RegDoctorDataModel [FromForm]"></param>
        /// <returns>RegDoctorDataModel model</returns>
        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> RegDoctorData([FromForm] RegDoctorDataModel model)
        {
            int result = 0;
            if (model.Uploads == null || model.Uploads.Count == 0)
            {
                model.SystemInfo = "<b>You have not uploaded files. Try again, please</b>";
                return BadRequest(model);
            }
            var UserIdClaim = User.FindFirst("MUId");
            var userId = UserIdClaim?.Value;
            try
            {
                if (Guid.TryParse(userId, out Guid UId) && model.Password != null)
                {
                    var pwdHash = _md5.CreateMd5(model.Password);
                    var pwdCheckRes = await _mediator.Send(new CheckUserPwdHashByUserIdQuery()
                    { UserId = UId, passwordHash = pwdHash });
                    if (pwdCheckRes==true)
                    {
                        result = await _mediator.Send(new AddRoleByNameToUserCommand() { UserId = UId, RoleName = "Applicant" });

                        List<FileDataDto> list = new();
                        if (result > 0)
                            model.SystemInfo = "<b>You have been added to applicants<br/>";
                        else
                            model.SystemInfo = "<b>You have been added to applicants before<br/>";

                        string uploadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads/files/cv/");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        foreach (var uploadedFile in model.Uploads)
                        {
                            string ext = Path.GetExtension(uploadedFile.FileName);
                            string name = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + $"-{DateTime.Now:HH.mm-dd.MM.yyyy}" + ext;
                            string path = "uploads/files/cv/" + name;
                            using (var fileStream = new FileStream(uploadPath + name, FileMode.Create))
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
                        var fileResult = await _mediator.Send(new AddListToFileDataCommand(){FileList = list});

                        if (fileResult > 1)
                            model.SystemInfo = model.SystemInfo + $"{fileResult} files were uploaded</b>";
                        else if (fileResult == 1)
                            model.SystemInfo = model.SystemInfo + $"1 file was uploaded</b>";
                        else
                            model.SystemInfo = model.SystemInfo + $"No files were uploaded</b>";

                        return Ok(model);
                    }
                    model.SystemInfo = "<b>You have entered incorrect password</b>";
                    return BadRequest(model);
                }
                model.SystemInfo = "<b>Something went wrong (</b>";
                return BadRequest(model);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        private async Task<EditDoctorDataModel> DoctorDataModelBuildAsync(EditDoctorDataModel model, Guid Uid)
        {
            model.Specialities = await _mediator.Send(new GetSpecialitiesListQuery());
            StringBuilder sb = new();
            string init = "<b>Marked for deletion specialities:<br/>";
            sb = sb.Append(init);
            model.UserId = Uid;
            if (model.Specialities != null)
            {
                var doctorData = await _mediator.Send(new GetDoctorDataListByUserIdQuery() { UserId=Uid });
                if (doctorData != null)
                {
                    foreach (var item in doctorData)
                    {
                        var spec = model.Specialities.FirstOrDefault(sp => sp.Id.Equals(item.SpecialityId));
                        if (spec != null && item.ForDeletion != true) spec.IsSelected = true;
                        if (spec != null && item.ForDeletion == true) sb.Append(spec.Name + "<br/>");
                    }
                }
            }
            if (sb.Length > init.Length)
            {
                sb.Append("</b>");
                model.SystemInfo = sb.ToString();
            }
            return model;
        }

    }
}
