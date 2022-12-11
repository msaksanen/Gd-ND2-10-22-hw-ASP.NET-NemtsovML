using AutoMapper;
using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactApp.Helpers;
using MedContactApp.Models;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Data;
using System.Security.Claims;

namespace MedContactApp.Controllers
{
    public class DayTimeTableController : Controller
    {
        private readonly IDayTimeTableService _dayTimeTableService;
        private readonly IMapper _mapper;
        private readonly IDoctorDataService _doctorDataService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private int _pageSize = 7;

        public DayTimeTableController(IDayTimeTableService dayTimeTableService, IConfiguration configuration,
            IMapper mapper, IDoctorDataService doctorDataService, ModelUserBuilder modelBuilder, IUserService userService)
        {
            _dayTimeTableService = dayTimeTableService;
            _mapper = mapper;
            _configuration = configuration;
            _doctorDataService = doctorDataService;
            _modelBuilder = modelBuilder;
            _userService = userService; 
        }


        [HttpGet]
        [Authorize(Roles = "Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> SelelctSpec(int fl=1)
        {
           try
           {
                var usr = await _modelBuilder.BuildUserById(HttpContext);
                if (usr == null) 
                    //return NotFound();
                      return NotFound("User is not found");
                DoctorSelectSpecModel model = new();
                model.User = usr;

                var dDataList = await _doctorDataService.GetDoctorInfoByUserId((Guid)usr.Id!);

                if (dDataList == null || dDataList.Count == 0)
                {
                    model.SystemInfo = @"<b>You do not have any specialities<br/>
                                     You can add speciality at ""Edit Doctor Data"" menu in your settings </b>";

                }

                else if (dDataList != null && (dDataList.All(dd => dd.SpecialityId == null) || dDataList.All(dd => dd.IsBlocked == true)))
                {
                    model.SystemInfo = @"<b>You do not have any active specialities<br/>
                                     Contact with administration, please </b>";

                }
                else if (dDataList != null && (dDataList.Any(dd => dd.SpecialityId == null) || dDataList.Any(dd => dd.IsBlocked == true) ||
                         dDataList.Any(dd => dd.ForDeletion == true)))
                {
                    model.SystemInfo = @"<b>You have some inactive/removed specialities<br/>
                                     Contact with administration for details, please </b>";
                    model.DoctorInfos = dDataList;
                }
                else
                {
                    model.DoctorInfos = dDataList;
                }
                if (fl == 2)
                    model.Flag = 2;

                return View(model);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        [HttpGet]
        public async Task<IActionResult> TimeTableDoctIndex(string? dataid, string? uid, string? reflink = "", SortState sortOrder = SortState.DateDesc, int page = 1 )
        {
            if (string.IsNullOrEmpty(dataid))
                //return BadRequest();
                 return new BadRequestObjectResult("DoctorData Id is null");
            var res = Guid.TryParse(dataid, out Guid dataId);
            if (res == false)
                //return BadRequest();
                 return new BadRequestObjectResult("DoctorData Id is incorrect");
            try
            {
                UserDto customer = new();
                var dInfo = await _doctorDataService.GetDoctorInfoById(dataId);
                if (dInfo == null || dInfo.DoctorDataId == null)
                    //return NotFound();
                      return NotFound("Doctor Data/Info is not found");
                IEnumerable<DayTimeTableDto>? timeTableList = await _dayTimeTableService.GetDayTimeTableByDoctorDataId((Guid)dInfo.DoctorDataId);

                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                int flag = 0;

                if (!string.IsNullOrEmpty(reflink) && (reflink?.Contains(@"daytimetable/selelctspec", StringComparison.OrdinalIgnoreCase) == true
                        || reflink?.Contains(@"adminpaneldoctor/doctordataindex", StringComparison.OrdinalIgnoreCase) == true)
                        || reflink?.Contains(@"appointment/viewindex", StringComparison.OrdinalIgnoreCase) == true
                        || reflink?.Contains(@"appointment/patientviewindex", StringComparison.OrdinalIgnoreCase) == true
                        || reflink?.Contains(@"daytimetable/create", StringComparison.OrdinalIgnoreCase) == true
                        || reflink?.Contains(@"appointment/createindex", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (User.Identities.Any(identity => identity.IsAuthenticated))
                    {
                        var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                        if (roles != null && roles.Any(r => r.Equals("Admin")))
                            flag = 1;
                        if (roles != null && roles.Any(r => r.Equals("Doctor")))
                            flag += 2;
                        if (reflink?.Contains(@"appointment/viewindex", StringComparison.OrdinalIgnoreCase) == true)
                            flag += 10;
                        if ((reflink?.Contains(@"appointment/patientviewindex", StringComparison.OrdinalIgnoreCase) == true
                            || reflink?.Contains(@"appointment/createindex", StringComparison.OrdinalIgnoreCase) == true
                            || reflink?.Contains(@"daytimetable/create", StringComparison.OrdinalIgnoreCase) == true) &&
                            !string.IsNullOrEmpty(uid) && Guid.TryParse(uid, out Guid guid))
                        {
                            flag += 20;
                            customer = await _userService.GetUserByIdAsync(guid);
                        }
                    }
                }
                else
                {
                    if (timeTableList != null)
                        timeTableList = timeTableList.Where
                                      (ttd => ttd?.Date!.Value != null && ttd?.Date!.Value! >= DateTime.Now.Date);
                }

                ViewData["Flag"] = flag;

                if (sortOrder == SortState.DateAsc && timeTableList != null)
                {
                    timeTableList = timeTableList.OrderBy(x => x.Date);
                }
                if (sortOrder == SortState.DateDesc && timeTableList != null)
                {
                    timeTableList = timeTableList.OrderByDescending(x => x.Date);
                }
                var items = timeTableList;
                int count = 0;
                if (timeTableList != null)
                {
                    count = timeTableList.Count();
                    items = timeTableList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }

                if (!string.IsNullOrEmpty(reflink))
                    reflink = reflink.Replace("*", "&");


                string pageRoute = @"/daytimetable/timetabledoctindex?page=";
                string processOptions = $"&dataid={dataid}&uid={uid}&sortorder={sortOrder}&reflink={reflink}";

                TimeTableDoctIndexModel model = new(
                       items, processOptions, dInfo, reflink,
                       new PageViewModel(count, page, pageSize, pageRoute),
                       new SortViewModel(sortOrder));

                if (customer != null)
                    model.User = customer;

                return View(model);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }


        //[HttpGet]
        //[Authorize(Policy = "FullBlocked")]
        //public async Task<IActionResult> Index(int page)
        //{
        //    try
        //    {
        //        bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
        //        if (result) _pageSize = pageSize;

        //        var DttDtoList = await _dayTimeTableService
        //             .GetDayTimeTableByPageNumberAndPageSizeAsync(page, _pageSize);

        //        if (DttDtoList.Any())
        //        {
        //            var count = await _dayTimeTableService.GetDayTimeTableEntitiesCountAsync();
        //            int pageCount = (int)Math.Ceiling((double)(count / _pageSize)) + 1; 
        //            ViewBag.pageCount = pageCount;
        //            List <DayTimeTableModel> modelList = new();
        //            for (int i = 0; i < DttDtoList.Count; i++) 
        //            {
        //                var doctDto = await _doctorDataService.GetDoctorInfoById(DttDtoList[i].DoctorDataId);
        //                var combiModel = _mapper.Map<DayTimeTableModel>((DttDtoList[i], doctDto));
        //                modelList.Add(combiModel);
        //            }
        //            return View(modelList);
        //        }
        //        else
        //        {
        //            throw new ArgumentException(nameof(page));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
        //        return BadRequest();
        //    }
        //}

        [HttpGet]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public  async Task<IActionResult> Create (string? dataid, string? uid)
        {
            if (string.IsNullOrEmpty(dataid))
                //return BadRequest();
                return new BadRequestObjectResult("DoctorData Id is null");
            var res = Guid.TryParse(dataid, out Guid dataId);
            if (res == false)
                //return BadRequest();
                return new BadRequestObjectResult("DoctorData Id is incorrect");
            try
            {
                var dData = await _doctorDataService.GetDoctorInfoById((Guid)dataId);
                if (dData == null)
                    //return NotFound();
                    return NotFound("Doctor info is not found");

                var model = _mapper.Map<DayTimeTableModel>(dData);

                if(!string.IsNullOrEmpty(uid) && Guid.TryParse(uid, out Guid dataUid))
                    model.CustomerUserId = dataUid;
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> Create (DayTimeTableModel model)
        {
            if (model == null || model.DoctorDataId == null)
                //return BadRequest();
                  return new BadRequestObjectResult("Model/DoctorData Id is null");
            try
            {  
                var dInfo = await _doctorDataService.GetDoctorInfoById(model.DoctorDataId);
                if (dInfo == null)
                    //return NotFound();
                   return NotFound("Doctor info is not found");

                model.Id = Guid.NewGuid();
                model.CreationDate = DateTime.Now;
                model.DoctorSpeciality = dInfo.Speciality;
                model.DoctorName = dInfo.Name;
                model.DoctorSurname = dInfo.Surname;
                model.UserId = dInfo.UserId;
                if (model.StartWorkTime!=null && model.FinishWorkTime!=null)
                {
                    model!.StartWorkTime = model?.Date?.Date!.Add((TimeSpan)model?.StartWorkTime!.Value.TimeOfDay!);
                    model!.FinishWorkTime = model?.Date?.Date!.Add((TimeSpan)model?.FinishWorkTime!.Value.TimeOfDay!);
                }
                
                
                var dto = _mapper.Map<DayTimeTableDto>(model);
                model!.SystemInfo = "<b>DayTimeTable was not created<br/>Something went wrong(</b>";

                if (model?.Date!.Value !=null && model?.Date!.Value! < DateTime.Now.Date)
                {
                    model.SystemInfo = "<b>Daytimetable was not created<br/>Input correct date!</b>";
                    return View(model);
                }

                if (dto != null)
                {
                    var result = await _dayTimeTableService.CreateDayTimeTableAsync(dto);
                    if (result > 0)
                    {
                        model!.SystemInfo = "<b>Daytimetable was successfully created</b>";
                    }
                    else if (result == -1)
                    {
                        model!.SystemInfo = "<b>DayTimeTable was not created<br/>It overlaps with previously created daytimetable</b>";
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

            return View(model);
        }
    }
}
