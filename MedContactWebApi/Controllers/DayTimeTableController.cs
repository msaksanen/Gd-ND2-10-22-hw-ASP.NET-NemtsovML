using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DayTimeTable.Commands;
using MedContactDataCQS.DayTimeTable.Queries;
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
    /// DayTimeTableController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DayTimeTableController : ControllerBase
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
        private int _pageSize = 7;

        /// <summary>
        /// DayTimeTable Controller Ctor
        /// </summary>
        public DayTimeTableController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, 
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
        /// SelelctSpeciality to make/view daytimetable 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> SelelctSpec()
        {
            try
            {
                var usr = await _modelBuilder.BuildUserDtoById(HttpContext);
                if (usr == null)
                    return NotFound("User is not found");
                DoctorSelectSpecModel model = new();
                model.User = usr;

                var dDataList = await _mediator.Send(new GetDoctorInfoByUserIdQuery() { UserId = usr.Id });

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

                return Ok(model);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        /// TimeTableDoctIndex
        /// </summary>
        /// <param name="model TimeTableDoctIndexRqstModel [FromQuery]"></param>
        /// string? dataid, string? reflink = "", SortState sortOrder = SortState.DateDesc, int page = 1
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TimeTableDoctIndex([FromQuery] TimeTableDoctIndexRqstModel model)
        {
            if (string.IsNullOrEmpty(model.Dataid))
                return BadRequest("DoctorData Id is null");
            var res = Guid.TryParse(model.Dataid, out Guid dataId);
            if (res == false)
                return BadRequest("DoctorData Id is incorrect");
            try
            {
                UserDto? customer = new();
                var dInfo = await _mediator.Send(new GetDoctorInfoByDocDataIdQuery() { DoctorDataId = dataId });
                if (dInfo == null || dInfo.DoctorDataId == null)
                    return NotFound("Doctor Data/Info is not found");
                IEnumerable<DayTimeTableDto>? timeTableList = await _mediator.Send(new GetDayTimeTableListByDoctorDataIdQuery() 
                                                                    { DoctorDataId = dInfo.DoctorDataId });

                if (model.PageSize!=null && model.PageSize>0)
                {
                    _pageSize = (int)model.PageSize;
                }
                else
                {
                    bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                    if (result) _pageSize = pageSize;
                }
                int flag = 0;


                if (!string.IsNullOrEmpty(model.RefLink) && (User.IsInRole("Doctor") || User.IsInRole("Admin")) &&
                    (model.RefLink?.Contains(@"daytimetable/selelctspec", StringComparison.OrdinalIgnoreCase) == true ||
                     model.RefLink?.Contains(@"appointment/patientviewindex", StringComparison.OrdinalIgnoreCase) == true ||
                     model.RefLink?.Contains(@"adminpaneldoctor/doctordataindex", StringComparison.OrdinalIgnoreCase) == true
                    ))
                {
                    if (User.IsInRole("Doctor") && model.RefLink?.Contains(@"daytimetable/selelctspec", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        flag = 1;  //doctor's daytimetable menu
                    }
                    if (User.IsInRole("Doctor") && model.RefLink?.Contains(@"appointment/patientviewindex", StringComparison.OrdinalIgnoreCase) == true &&
                         !string.IsNullOrEmpty(model.Uid) && Guid.TryParse(model.Uid, out Guid guid))
                    {
                        flag = 2; //doctor's meddata menu && re-appointment of patients
                        customer = await _mediator.Send(new GetUserByIdQuery() { UserId = guid });
                    }
                    if (User.IsInRole("Admin") && model.RefLink?.Contains(@"adminpaneldoctor/doctordataindex", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        flag = 3; //admin menu doctordataindex
                    }
                }
                else
                {
                    if (timeTableList != null)
                        timeTableList = timeTableList.Where
                                      (ttd => ttd?.Date!.Value != null && ttd?.Date!.Value! >= DateTime.Now.Date);
                    flag = 0; //customer menu
                }

                //if (!string.IsNullOrEmpty(model.RefLink) && (model.RefLink.Contains(@"daytimetable/selelctspec", StringComparison.OrdinalIgnoreCase) == true
                //        || model.RefLink.Contains(@"adminpaneldoctor/doctordataindex", StringComparison.OrdinalIgnoreCase) == true)
                //        || model.RefLink.Contains(@"appointment/viewindex", StringComparison.OrdinalIgnoreCase) == true)
                //{
                //    if (User.Identities.Any(identity => identity.IsAuthenticated))
                //    {
                //        var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                //        if (roles != null && roles.Any(r => r.Equals("Admin")))
                //            flag = 1;
                //        if (roles != null && roles.Any(r => r.Equals("Doctor")))
                //            flag += 2;
                //        if (model.RefLink.Contains(@"appointment/viewindex", StringComparison.OrdinalIgnoreCase) == true)
                //            flag += 10;
                //    }
                //}
                //else
                //{
                //    if (timeTableList != null)
                //        timeTableList = timeTableList.Where
                //                      (ttd => ttd?.Date!.Value != null && ttd?.Date!.Value! >= DateTime.Now.Date);
                //}

                //ViewData["Flag"] = flag;

                if (model.SortOrder == SortState.DateAsc && timeTableList != null)
                {
                    timeTableList = timeTableList.OrderBy(x => x.Date);
                }
                if (model.SortOrder == SortState.DateDesc && timeTableList != null)
                {
                    timeTableList = timeTableList.OrderByDescending(x => x.Date);
                }
                var items = timeTableList;
                int count = 0;
                if (timeTableList != null)
                {
                    count = timeTableList.Count();
                    items = timeTableList.Skip((model.Page - 1) * _pageSize).Take(_pageSize).ToList();
                    items.ToList().ForEach(d => d.DoctorData = null);
                }

                if (!string.IsNullOrEmpty(model.RefLink))
                    model.RefLink = model.RefLink.Replace("*", "&");


                string pageRoute = @"/daytimetable/timetabledoctindex?page=";
                string processOptions = $"&dataid={model.Dataid}&uid={model.Uid}&sortorder={model.SortOrder}&reflink={model.RefLink}";

                TimeTableDoctIndexModel viewmodel = new(
                       items, processOptions, dInfo, model.RefLink, flag,
                       new PageViewModel(count, model.Page, _pageSize, pageRoute),
                       new SortViewModel(model.SortOrder));

                if (customer != null)
                    viewmodel.User = customer;
                return Ok(viewmodel);

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }


        /// <summary>
        /// Create  DayTimeTable
        /// </summary>
        /// <param name="dataid  string? [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> Create([FromQuery] string? dataid)
        {
            if (string.IsNullOrEmpty(dataid))
                return BadRequest("DoctorData Id is null");
            var res = Guid.TryParse(dataid, out Guid dataId);
            if (res == false)
                return BadRequest("DoctorData Id is incorrect");
            try
            {
                var dData = await _mediator.Send(new GetDoctorInfoByDocDataIdQuery() { DoctorDataId = dataId });
                if (dData == null)
                    return NotFound("Doctor info is not found");

                var model = _mapper.Map<DayTimeTableModel>(dData);
                return Ok(model);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        ///  Create  DayTimeTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> Create([FromBody]DayTimeTableModel model)
        {
            if (model == null || model.DoctorDataId == null)
                return BadRequest ("Model/DoctorData Id is null");
            try
            {
                var dInfo = await _mediator.Send(new GetDoctorInfoByDocDataIdQuery() { DoctorDataId = model.DoctorDataId });
                if (dInfo == null)
                    return NotFound("Doctor info is not found");

                model.Id = Guid.NewGuid();
                model.CreationDate = DateTime.Now;
                model.DoctorSpeciality = dInfo.Speciality;
                model.DoctorName = dInfo.Name;
                model.DoctorSurname = dInfo.Surname;
                model.UserId = dInfo.UserId;
                if (model.StartWorkTime != null && model.FinishWorkTime != null)
                {
                    model!.StartWorkTime = model?.Date?.Date!.Add((TimeSpan)model?.StartWorkTime!.Value.TimeOfDay!);
                    model!.FinishWorkTime = model?.Date?.Date!.Add((TimeSpan)model?.FinishWorkTime!.Value.TimeOfDay!);
                }
                else
                {
                    model.SystemInfo = "<b>Daytimetable was not created<br/>Start or Finish WorkTime are incorrect</b>";
                    return BadRequest(model);
                }


                var dto = _mapper.Map<DayTimeTableDto>(model);
                model!.SystemInfo = "<b>DayTimeTable was not created<br/>Something went wrong(</b>";

                if (model?.Date!.Value != null && model?.Date!.Value! < DateTime.Now.Date)
                {
                    model.SystemInfo = "<b>Daytimetable was not created<br/>Input correct date!</b>";
                    return BadRequest(model);
                }

                if (dto != null)
                {
                    var result = await _mediator.Send(new CreateDayTimeTableCommand(){ dayTimeTableDto = dto });
                    if (result.IntResult > 0 && result.IntResult1!=null && result.IntResult2!=null)
                    {
                        model!.SystemInfo = "<b>Daytimetable was successfully created</b>";
                        model.TotalTicketQty = result.IntResult1;
                        model.FreeTicketQty = result.IntResult2;
                        return Ok(model);
                    }
                    else if (result.IntResult == -1)
                    {
                        model!.SystemInfo = "<b>DayTimeTable was not created<br/>It overlaps with previously created daytimetable</b>";
                        return BadRequest (model); 
                    }
                }

                model!.SystemInfo = "<b>DayTimeTable was not created<br/>Something went wrong(</b>";
                return BadRequest();

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }
    }
}
