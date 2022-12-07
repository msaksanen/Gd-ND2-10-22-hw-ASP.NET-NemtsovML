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
        private int _pageSize = 7;

        public MedDataController(IUserService userService, IConfiguration configuration,
            IMapper mapper, ModelUserBuilder modelBuilder, IMedDataService medDataService,
            IFileDataService fileDataService, IFamilyService familyService, MedDataSortFilter medSortFilter, 
            ICustomerDataService customerDataService, IWebHostEnvironment appEnvironment, FileValidation fileValidation)
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
        }


        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> UserMedData(string id,string type, string speciality, string name, string date, 
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
                IQueryable<MedData> medDatas = _medDataService.GetMedData()
                                               .Where(m => m.CustomerData != null && m.CustomerData.UserId.Equals(uId))
                                               .Include(m => m.DoctorData)
                                               .ThenInclude(d => d!.Speciality)
                                               .Include(m => m.DoctorData)
                                               .ThenInclude(d => d!.User);

                medDatas = _medSortFilter.MedFilter(medDatas, type, speciality, name,
                                                    date, depart, text);

                medDatas = _medSortFilter.MedDataSort(medDatas, sortOrder);

                var count = await medDatas.CountAsync();
                var items = await medDatas.Skip((page - 1) * pageSize).Take(pageSize)
                         .Select(md => _mapper.Map<MedDataDto>(md)).ToListAsync();


                var datainfo = items?.Select(x => _mapper.Map<MedDataInfo>
                                          ((x, x?.DoctorData, x?.CustomerData ?? new CustomerDataDto(), usr)));

                string pageRoute = @"/meddata/usermeddata?page=";
                string processOptions = $"&id={id}&type={type}&speciality={speciality}&name={name}&date={date}&depart={depart}&text={text}&sortorder={sortOrder}";

                string link = Request.Path.Value + Request.QueryString.Value;
                if (string.IsNullOrEmpty(Request.QueryString.Value))
                    link +=@$"/?id={usr.Id}";
                else
                    link += @$"&id={usr.Id}";
                link = link.Replace("&", "*");
                ViewData["Reflink"] = link;
                MedDataIndexViewModel viewModel = new(
                    uId, sysinfo, datainfo, processOptions,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterMedDataViewModel(name, type, speciality, depart, text, date),
                    new SortViewModel(sortOrder)
                );
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
        public async Task<IActionResult> AddOrEditMedData(string id, string mdataid, string? reflink = "")
        {
            Guid uId = Guid.Empty;
            int flag = 0;
            if (!string.IsNullOrEmpty(reflink))
                reflink = reflink.Replace("*", "&");
            try
            {  if (!string.IsNullOrEmpty(mdataid) && !string.IsNullOrEmpty(id))
               {
                    flag = 2;
                    var model = await CreateUserMedDataModel(mdataid, id);
                    if (model.ErrorObject != null)
                        return (IActionResult)model.ErrorObject;
                    if (model.MedDataType.Any())
                    {
                        var item = model.MedDataType.FirstOrDefault(i => i.Name != null && i.Name.Equals(model.Type, StringComparison.OrdinalIgnoreCase));
                        if (item != null)
                            item.IsSelected = true;
                    }
                    model.Reflink = reflink;
                    model.Flag = flag;
                    return View(model);
                }

                if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid gId))
                    uId = gId;
                else
                    return new BadRequestObjectResult("User Id is incorrect");

                var usr = await _userService.GetUserByIdAsync(uId);
                if (usr == null)
                    return NotFound("User is not found");

                if (!string.IsNullOrEmpty(reflink) && (reflink?.Contains(@"meddata/usermeddata", StringComparison.OrdinalIgnoreCase) == true))
                    flag = 1;
                if (string.IsNullOrEmpty(reflink))
                    flag = 3;
                //if (!string.IsNullOrEmpty(reflink) && (reflink?.Contains(@"meddata/doctormeddata", StringComparison.OrdinalIgnoreCase) == true) &&
                //    User.IsInRole("Doctor"))
                //    flag = 2;
                if (flag == 0)
                    return new BadRequestObjectResult("Incorrect route");
                

                if (flag == 1 || flag==3)
                {
                    var custData = await _customerDataService.GetOrCreateByUserIdAsync(uId);
                    if (custData == null)
                        return NotFound("Customer data is not found/ not created");

                    var model = _mapper.Map<UserMedDataModel>(usr);
                    model.CustomerDataId = custData.Id;
                    model.Reflink = reflink;
                    model.Flag = flag;
                    return View(model); 
                }
              
                    return new BadRequestObjectResult("Not Implemented");
                
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

                var custData = await _customerDataService.GetOrCreateByUserIdAsync(usr.Id);
                if (custData == null)
                    return NotFound("Customer data is not found/ not created");

                MedDataDto medData = new()
                {
                    Id = Guid.NewGuid(),
                    CustomerDataId = custData.Id,
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

                if(model.MedDataId!=null)
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
                        model.SystemInfo += "<b>MedData has been modified.</b>";
                    else
                        model.SystemInfo += "<b>MedData has not been modified.</b>";
                }
                else
                {
                    var res = await _medDataService.AddMedDataInDbAsync(medData);
                    if (res > 0)
                        model.SystemInfo += "<b>MedData has been saved.</b>";
                    else
                        model.SystemInfo += "<b>MedData has not been saved.</b>";
                }

                if (model.Uploads == null || model.Uploads.Count == 0)
                    model.SystemInfo += "<br/><b>You have not uploaded files</b>";
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

                return RedirectToAction("UserMedData", "MedData", new { sysinfo = model.SystemInfo });
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> UserMedDataDetails(string mdataid, string id, string filedata, string reflink="")
        {

            string sysInfo = string.Empty;  
            try
            {
                var model = await CreateUserMedDataModel(mdataid, id);
                if (model.ErrorObject != null)

                    return (IActionResult)model.ErrorObject;


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
                return View(model);
            }
           
             catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
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
