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
using MedContactApp.FilterSortHelpers;
using MedContactApp.AdminPanelHelpers;
using MedContactDb.Entities;
using System.Data;
using System.Drawing;
using MedContactApp.FilterSortPageHelpers;
using System.Collections.Generic;
using MedContactBusiness.ServicesImplementations;

namespace MedContactApp.Controllers
{
    public class MedDataController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFamilyService _familyService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly MedDataSortFilter _medSortFilter;
        private readonly IMedDataService _medDataService;
        private readonly IFileDataService _fileDataService;
        private readonly ICustomerDataService _customerDataService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly FileValidation _fileValidation;
        private readonly IDoctorDataService _doctorDataService;
        private int _pageSize = 7;

        public MedDataController(IUserService userService, IConfiguration configuration,
            IMapper mapper, ModelUserBuilder modelBuilder, IMedDataService medDataService,
            IFileDataService fileDataService, IFamilyService familyService, MedDataSortFilter medSortFilter, 
            ICustomerDataService customerDataService, IWebHostEnvironment appEnvironment, FileValidation fileValidation, 
            IDoctorDataService doctorDataService)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _familyService = familyService;
            _modelBuilder = modelBuilder;
            _medSortFilter = medSortFilter;
            _medDataService = medDataService;
            _fileDataService = fileDataService;
            _customerDataService = customerDataService;
            _appEnvironment = appEnvironment;
            _fileValidation = fileValidation;
            _doctorDataService = doctorDataService;
            }


        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> UserMedData(string id, string doctid,string type, string speciality, string name, string date, 
            string depart, string text, string sysinfo, int page = 1, SortState sortOrder = SortState.DateDesc)
        {
            Guid uId = Guid.Empty;
            try 
            {
                if (string.IsNullOrEmpty(id))
                {
                    var UserIdClaim = User.FindFirst("MUId");
                    if (UserIdClaim?.Value != null && Guid.TryParse(UserIdClaim.Value, out Guid guid))
                        uId = guid;
                    else
                        return NotFound("Main user Id is not found");
                }
                else if (Guid.TryParse(id, out Guid gId))
                        uId = gId;
                    else
                        return new BadRequestObjectResult("User Id is incorrect");

               

                var usr = await _userService.GetUserByIdAsync(uId);
                if (usr==null)
                       return NotFound("User is not found");     

                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
            
                var medDatas = _medDataService.GetUserMedData(uId);

                medDatas = _medSortFilter.MedFilter(medDatas, type, speciality, name,
                                                    date, depart, text);

                medDatas = _medSortFilter.MedDataSort(medDatas, sortOrder);

                var count = await medDatas.CountAsync();
                var items = await medDatas.Skip((page - 1) * pageSize).Take(pageSize)
                         .Select(md => _mapper.Map<MedDataDto>(md)).ToListAsync();


                var datainfo = items?.Select(x => _mapper.Map<MedDataInfo>
                                          ((x, x?.DoctorData, x?.CustomerData ?? new CustomerDataDto(), usr)));

                string pageRoute = @"/meddata/usermeddata?page=";
                string processOptions = $"&id={id}&doctid={doctid}&type={type}&speciality={speciality}&name={name}&date={date}&depart={depart}&text={text}&sortorder={sortOrder}";

                string link = Request.Path.Value + Request.QueryString.Value;
                //if (string.IsNullOrEmpty(Request.QueryString.Value))
                //    link +=@$"/?id={usr.Id}&doctid={doctid}";
                //else
                //    link += @$"&id={usr.Id}&doctid={doctid}";
                link = link.Replace("&", "*");
                ViewData["Reflink"] = link;
                MedDataIndexViewModel viewModel = new(
                    usr, sysinfo, link, datainfo, processOptions,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterMedDataViewModel(name, type, speciality, depart, text, date),
                    new SortViewModel(sortOrder)
                );
                if (!string.IsNullOrEmpty(doctid) && Guid.TryParse(doctid, out Guid dId))
                {
                    viewModel.DoctId = doctid;         
                }
                  
                return View(viewModel);

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }


        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> AddOrEditMedData(string id, string mdataid, string doctid, string? reflink = "")
        {
            Guid uId = Guid.Empty;
            int flag = 0;
            if (!string.IsNullOrEmpty(reflink))
                reflink = reflink.Replace("*", "&");
            try
            {
                //if (!string.IsNullOrEmpty(doctid) && Guid.TryParse(doctid, out Guid dId))
                //    ViewData["DoctId"] = doctid;

                if (!string.IsNullOrEmpty(mdataid) && !string.IsNullOrEmpty(id)) //model for edit
                {
                   
                    var model = await CreateUserMedDataModel(mdataid, id);
                    
                    if (model.ErrorObject != null)
                        return (IActionResult)model.ErrorObject;
                    model.Reflink = reflink;
                    model.Flag = 2;

                    if (model.MedDataType.Any())
                    {
                        var item = model.MedDataType.FirstOrDefault(i => i.Name != null && i.Name.Equals(model.Type, StringComparison.OrdinalIgnoreCase));
                        if (item != null)
                            item.IsSelected = true;
                    }
               
                    if (await UserAccessCheck(model)>0)
                             return View(model);
                    else 
                        return RedirectToAction("UserMedData", "MedData",
                               new { id, doctid, sysinfo = "<b>You have no access to modify foreign data.</b>" });

                }

                if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid gId)) //meddata create
                    uId = gId;
                else
                    return new BadRequestObjectResult("User Id is incorrect");

                var usr = await _userService.GetUserByIdAsync(uId);
                if (usr == null)
                    return NotFound("User is not found");

                if (!string.IsNullOrEmpty(reflink))
                    flag = 1;
                if (string.IsNullOrEmpty(reflink))
                    flag = 3;

                if (flag == 1 || flag==3)
                {
                    var custData = await _customerDataService.CreateByUserIdAsync(uId);
                    if (custData == null)
                        return NotFound("Customer data is not found/ not created");

                    var model = _mapper.Map<UserMedDataModel>(usr);
                    if (!string.IsNullOrEmpty(doctid) && Guid.TryParse(doctid, out Guid doctId))
                    {
                       if(await _doctorDataService.IsCorrectDoctorId(doctId))
                          model.DoctorDataId = doctId;
                    }

                    model.CustomerDataId = custData.Id;
                    model.Reflink = reflink;
                    model.Flag = flag;
                    return View(model); 
                }

                return new BadRequestObjectResult("Incorrect route");
            }
             catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> AddOrEditMedData(UserMedDataModel model)
        {
           try
           {
                if (model.UserId == null)
                {
                    return new BadRequestObjectResult("User Id is incorrect");
                }
                var usr = await _userService.GetUserByIdAsync((Guid)model.UserId);
                if (usr == null)
                    return NotFound("User is not found");

                var custData = await _customerDataService.CreateByUserIdAsync(usr.Id);
                if (custData == null)
                    return NotFound("Customer data is not found/ not created");

                MedDataDto medData = new()
                {
                    Id = Guid.NewGuid(),
                    CustomerDataId = custData.Id,
                    DoctorDataId = model.DoctorDataId,
                    Department = model.Department,
                    InputDate = DateTime.Now,
                    TextData = model.TextData,
                    Type = "Other",
                    ShortSummary = model.ShortSummary
                };

                model.SystemInfo = String.Empty;

                if (model.MedDataTypeIds != null && model.MedDataTypeIds.Any())
                {
                    var item = model.MedDataType.FirstOrDefault(x => x.IntId.Equals(model.MedDataTypeIds[0]));
                    if (item != null)
                        medData.Type = item.Name;
                }

                if(model.MedDataId!=null) //data modification
                {
                    var sourceDto = await _medDataService.GetMedDataByIdAsync((Guid)model.MedDataId);
                    if (sourceDto == null)
                        return NotFound("Source MedData has not been found");
                    medData.Id = sourceDto.Id;
                    medData.InputDate = sourceDto.InputDate;
                    PatchMaker<MedDataDto> patchMaker = new();
                    var patchList = patchMaker.Make(medData, sourceDto);
                    var resPatch = await _medDataService.PatchAsync(sourceDto.Id, patchList);
                    if (resPatch > 0)
                        model.SystemInfo += "<b>MedData has been modified. </b>";
                    else
                        model.SystemInfo += "<b>MedData has not been modified. </b>";
                }
                else
                {
                    var res = await _medDataService.AddMedDataInDbAsync(medData);
                    if (res > 0)
                        model.SystemInfo += "<b>MedData has been saved. </b>";
                    else
                        model.SystemInfo += "<b>MedData has not been saved. </b>";
                }

                if (model.Uploads == null || model.Uploads.Count == 0)
                    model.SystemInfo += "<b> You have not uploaded files</b>";
                else
                {
                    var resSize = _fileValidation.FileSizeValidation(model.Uploads);
                    var resExt = _fileValidation.FileExtValidation(model.Uploads);
                    if (resSize.IntResult == 1 && resExt.IntResult == 1)
                    {
                        List<FileDataDto> list = new();

                        foreach (var uploadedFile in model.Uploads)
                        {
                            string ext = Path.GetExtension(uploadedFile.FileName);
                            string name = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + $"-{DateTime.Now:HH.mm-dd.MM.yyyy}" + ext;
                            string path = "/files/meddata/" + name;
                            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                            {
                                await uploadedFile.CopyToAsync(fileStream);
                            }
                            FileDataDto file = new FileDataDto
                            {
                                Id = Guid.NewGuid(),
                                MedDataId = medData.Id,
                                Name = uploadedFile.FileName,
                                Path = path,
                                Category = "MedData"
                            };
                            list.Add(file);
                        }
                        var fileResult = await _fileDataService.AddListToFileData(list);

                        if (fileResult > 1)
                            model.SystemInfo += $"<b> {fileResult} files have been uploaded.</b>";
                        else if (fileResult == 1)
                            model.SystemInfo += $"<b> 1 file has been uploaded.</b>";
                        else
                            model.SystemInfo += $"<b> No files have been uploaded./b>";
                    }
                    else
                        model.SystemInfo += $"<b> No files have been uploaded.</b>" + resSize.Name + resExt.Name;
                }

                return RedirectToAction("UserMedData", "MedData", new { id = usr.Id, doctid = model.DoctorDataId, sysinfo = model.SystemInfo });
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> UserMedDataDetails(string mdataid, string id, string doctid, string filedata, string reflink="")
        {

            string sysInfo = string.Empty;  
            try
            {
                var model = await CreateUserMedDataModel(mdataid, id);
                if (model.ErrorObject != null)

                    return (IActionResult)model.ErrorObject;

                if (!string.IsNullOrEmpty(doctid) && Guid.TryParse(doctid, out Guid dId))
                    ViewData["DoctId"] = doctid;

                if (!string.IsNullOrEmpty(filedata) && Guid.TryParse(filedata, out Guid fileId) &&
                    model.fileDatas != null && model.fileDatas.Any(f => f.Id.Equals(fileId)))
                {
                    var deleted = model.fileDatas.FirstOrDefault(f => f.Id.Equals(fileId));
                    var removeResult = await _fileDataService.RemoveFileDataWithFileById(fileId, _appEnvironment.WebRootPath);

                    switch (removeResult)
                    {
                        case -2:
                            sysInfo = $"<b>FileData with name ({deleted?.Name}) was not found <br/>File cannot be deleted</b>";
                            break;
                        case -1:
                            sysInfo = $"<b>FileData with name ({deleted?.Name}) has incorrect path <br/>File cannot be deleted</b>";
                            break;
                        case 1:
                            sysInfo = $"<b>FileData with name ({deleted?.Name}) was deleted <br/>The file was removed before</b>";
                            break;
                        case 2:
                            sysInfo = $"<b>FileData with name ({deleted?.Name}) and its file were deleted</b>";
                            break;
                    }
                    if (deleted != null && (removeResult == 1 || removeResult == 2))
                        model.fileDatas.Remove(deleted);
                }

                if (!string.IsNullOrEmpty(reflink))
                    reflink = reflink.Replace("*", "&");
                model.Reflink = reflink;
                model.SystemInfo = sysInfo;

                var res = await UserAccessCheck(model);

                if(res==2)   // access for modification
                    return View(model);
                
                if (res==1)
                {
                    model.Flag = 25; // access for removal and modification
                    return View(model);
                }


                model.Flag = 20;
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
        public async Task<IActionResult> DeleteMedData(string mdataid, string id, string doctid, string filedata, string reflink = "")
        {
            try
            {
                string sysInfo = string.Empty;
                var model = await CreateUserMedDataModel(mdataid, id);
                if (model.ErrorObject != null)
                    return (IActionResult)model.ErrorObject;
                var res = await UserAccessCheck(model);
                if (res != 1)
                {
                    return RedirectToAction("UserMedData", "MedData",
                               new { id, doctid, sysinfo = "<b>You have no access to delete foreign data.</b>" });
                }

                int delRes = 0;
                if (model.fileDatas != null && model.fileDatas.Any())
                {
                    foreach (var fileData in model.fileDatas)
                    {
                        var tempres = await _fileDataService.RemoveFileDataWithFileById(fileData.Id, _appEnvironment.WebRootPath);
                        if (tempres > 0) delRes++;
                    }
                    sysInfo = $"<b>{delRes} files have been deleted. </b>";
                }
                var delMed = await _medDataService.RemoveMedDataById((Guid)model.MedDataId!);
                if (delMed > 0)
                    sysInfo += $"<b>MedData has been deleted. </b>";
                else
                    sysInfo += $"<b>MedData has not been deleted.</b>";
              
                return RedirectToAction("UserMedData", "MedData", new { id, doctid, sysinfo=sysInfo});

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        private async Task<int> UserAccessCheck(UserMedDataModel model)
        {
            if (model.DoctorDataId == null && User.FindFirst("MUId")?.Value != null && Guid.TryParse(User.FindFirst("MUId")?.Value, out Guid uId))
            {
                var mainUsr = await _userService.GetUserByIdAsync(uId);
                if ((mainUsr != null && mainUsr.Id.Equals(model.UserId)) ||
                    (mainUsr != null && mainUsr.FamilyId != null && model.FamilyId != null && mainUsr.FamilyId.Equals(model.FamilyId)))
                {
                    return 1;
                }
            }

            if (model.DoctorDataId != null && User.IsInRole("Doctor") && User.FindFirst("MUId")?.Value != null &&
                                    Guid.TryParse(User.FindFirst("MUId")?.Value, out Guid dId))
            {
                var doclist = await _doctorDataService.GetDoctorInfoByUserId(dId);
                if (doclist != null && doclist.Any(d => d.DoctorDataId.Equals(model.DoctorDataId))) // meddata of current doctor
                    return 2;
            }
            return 0;
        }
        private async Task<UserMedDataModel> CreateUserMedDataModel (string mdataid, string id)
        {
            UserMedDataModel model = new();
            if (string.IsNullOrEmpty(id))
            {
                model.ErrorObject = new BadRequestObjectResult("User Id is null");
                return model;
            }
            var resU = Guid.TryParse(id, out Guid uId);
            if (!resU)
            {
                model.ErrorObject = new BadRequestObjectResult("User Id is incorrect");
                return model;
            }
            if (string.IsNullOrEmpty(mdataid))
            {
                model.ErrorObject = new BadRequestObjectResult("MedData Id is null");
                return model;
            }
            var resM = Guid.TryParse(mdataid, out Guid medId);
            if (!resM)
            {
                model.ErrorObject = new BadRequestObjectResult("MedData Id is incorrect");
                return model;
            }
            var medData = await _medDataService.GetMedDataByIdAsync(medId);
            if (medData == null)
            {
                model.ErrorObject = NotFound("MedData is not found");
                return model;
            }
            var usr = await _userService.GetUserByIdAsync(uId);
            if (usr == null)
            {
                model.ErrorObject = NotFound("User is not found");
                return model;
            }
            var fileDataList = await _fileDataService.GetFileDataListByMedDataIdAsync(medData.Id);
            var newModel = _mapper.Map<UserMedDataModel>((usr, medData, fileDataList));

            return newModel;
        }


    }
}
