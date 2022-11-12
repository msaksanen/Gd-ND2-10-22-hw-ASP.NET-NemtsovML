using Microsoft.AspNetCore.Mvc;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactApp.Models;
using AutoMapper;
using Serilog;
using System.ComponentModel.Design;
using System.Configuration;
using System;
using System.Reflection;
using System.Linq;
using MedContactApp.Helpers;
using MedContactDb.Entities;
using MedContactApp.FilterSortHelpers;
using System.Drawing.Printing;
using System.Xml.Linq;
using MedContactApp.FilterSortPageHelpers;
using System.Data;
using System.Drawing;
using System.Security.Claims;

namespace MedContactApp.Controllers
{
    public class DayTimeTableController : Controller
    {
        private readonly IDayTimeTableService _dayTimeTableService;
        private readonly IMapper _mapper;
        private readonly IDoctorDataService _doctorDataService;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private int _pageSize = 7;

        public DayTimeTableController(IDayTimeTableService dayTimeTableService, IConfiguration configuration,
            IMapper mapper, IDoctorDataService doctorDataService, ModelUserBuilder modelBuilder)
        {
            _dayTimeTableService = dayTimeTableService;
            _mapper = mapper;
            _configuration = configuration;
            _doctorDataService = doctorDataService;
            _modelBuilder = modelBuilder;
        }


        [HttpGet]
        public async Task<IActionResult> SelelctSpec()
        {
            var usr = await _modelBuilder.BuildUserById(HttpContext);
            if (usr==null ) return NotFound();
            DoctorSelectSpecModel model = new();
            model.User = usr;

            var dDataList = await _doctorDataService.GetDoctorInfoByUserId((Guid)usr.Id!);

            if (dDataList==null || dDataList.Count==0)
            {
                model.SystemInfo = @"<b>You do not have any specialities<br/>
                                     You can add speciality at ""Edit Doctor Data"" menu in your settings </b>";

            }

            else if (dDataList != null && (dDataList.All(dd => dd.SpecialityId == null) || dDataList.All(dd => dd.IsBlocked==true)))
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

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TimeTableDoctIndex(string? dataid, string? reflink = "", SortState sortOrder = SortState.DateDesc, int page = 1 )
        {
            if (string.IsNullOrEmpty(dataid))
                return BadRequest();
            var res = Guid.TryParse(dataid, out Guid dataId);
            if (res == false)
                return BadRequest();
            var dInfo = await _doctorDataService.GetDoctorInfoById(dataId);
            if (dInfo == null || dInfo.DoctorDataId == null)
                return NotFound();
            IEnumerable <DayTimeTableDto>? timeTableList = await _dayTimeTableService.GetDayTimeTableByDoctorDataId((Guid)dInfo.DoctorDataId);
           
            bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
            if (result) _pageSize = pageSize;

            if (sortOrder==SortState.DateAsc && timeTableList != null)
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
             
            int flag = 0;
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {
                var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                if (roles != null && roles.Any(r => r.Equals("Admin") || r.Equals("Doctor")) &&
                    !string.IsNullOrEmpty(reflink) && reflink?.Contains(@"daytimetable/selelctspec") == true
                    || reflink?.Contains(@"adminpaneldoctor/doctordataindex") == true)
                {
                    flag = 1;
                }
            }

            if (!string.IsNullOrEmpty(reflink))
                reflink = reflink.Replace("*", "&");
   

            string pageRoute = @"/daytimetable/timetabledoctindex?page=";
            string processOptions = $"&dataid={dataid}&sortorder={sortOrder}&reflink={reflink}";

            TimeTableDoctIndexModel model = new(
                   items, processOptions,dInfo, flag, reflink,
                   new PageViewModel(count, page, pageSize, pageRoute),
                   new SortViewModel(sortOrder));

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            try
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;

                var DttDtoList = await _dayTimeTableService
                     .GetDayTimeTableByPageNumberAndPageSizeAsync(page, _pageSize);

                if (DttDtoList.Any())
                {
                    var count = await _dayTimeTableService.GetDayTimeTableEntitiesCountAsync();
                    int pageCount = (int)Math.Ceiling((double)(count / _pageSize)) + 1; 
                    ViewBag.pageCount = pageCount;
                    List <DayTimeTableModel> modelList = new();
                    for (int i = 0; i < DttDtoList.Count; i++) 
                    {
                        var doctDto = await _doctorDataService.GetDoctorInfoById(DttDtoList[i].DoctorDataId);
                        var combiModel = _mapper.Map<DayTimeTableModel>((DttDtoList[i], doctDto));
                        modelList.Add(combiModel);
                    }
                    return View(modelList);
                }
                else
                {
                    throw new ArgumentException(nameof(page));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpGet]
        public  async Task<IActionResult> Create (string? dataid)
        {
            if (string.IsNullOrEmpty(dataid))
                return BadRequest();
            var res = Guid.TryParse(dataid, out Guid dataId);
            if (res == false)
                return BadRequest();
            var dData = await _doctorDataService.GetDoctorInfoById((Guid)dataId);
            if (dData == null)
                return NotFound();
            
                var model = _mapper.Map<DayTimeTableModel>(dData);
                return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create (DayTimeTableModel model)
        {
            if (model == null || model.DoctorDataId == null)
                return BadRequest();
            try
            {
               if (model.Date < DateTime.Now)
                {
                    model.SystemInfo = "<b>Daytimetable was not created<br/>Input correct date!</b>";
                    return View(model);
                }

                var dInfo = await _doctorDataService.GetDoctorInfoById(model.DoctorDataId);
                if (dInfo == null)
                    return NotFound();

                model.Id = Guid.NewGuid();
                model.CreationDate = DateTime.Now;
                model.DoctorSpeciality = dInfo.Speciality;
                model.DoctorName = dInfo.Name;
                model.DoctorSurname = dInfo.Surname;
                model.UserId = dInfo.UserId;    
                var dto = _mapper.Map<DayTimeTableDto>(model);
                model.SystemInfo = "<b>DayTimeTable was not created<br/>Something went wrong(</b>";

                if (dto != null)
                {
                    var result = await _dayTimeTableService.CreateDayTimeTableAsync(dto);
                    if (result > 0)
                    {
                        model.SystemInfo = "<b>Daytimetable was successfully created</b>";
                    }
                    else if (result == -1)
                    {
                        model.SystemInfo = "<b>DayTimeTable was not created<br/>It overlaps with previously created daytimetable</b>";
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
