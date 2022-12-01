using AutoMapper;
using MedContactApp.WebApi;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Appointment.Command;
using MedContactDataCQS.Appointment.Queries;
using MedContactDataCQS.CustomerData.Commands;
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
using System.Collections.Generic;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// AppointmentController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : ControllerBase
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
        /// Appointment Controller Ctor
        /// </summary>
        public AppointmentController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, 
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
        /// Appointment CreateIndex
        /// </summary>
        /// <param name="id [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> CreateIndex([FromQuery]string? id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("DayTimeTable Id is null");
            var res = Guid.TryParse(id, out Guid dttId);
            if (!res)
                return BadRequest("DayTimeTable Id is incorrect");
            try
            {
                var dttDto = await _mediator.Send(new GetDayTimeTableByIdQuery() { DttId = dttId });
                if (dttDto == null)
                    return NotFound("DayTimeTable is not found");
                var doctInfo = await _mediator.Send(new GetDoctorInfoByDocDataIdQuery() { DoctorDataId = dttDto.DoctorDataId });
                if (doctInfo == null)
                    return NotFound("Doctor info is not found");
                var usr = await _modelBuilder.BuildUserDtoById(HttpContext, flag: 2);
                if (usr == null)
                    return NotFound("User is not found");

                var customerData = await _mediator.Send(new GetOrCreateByUserIdCommand(){UserId =usr.Id });

                var apmList = await _mediator.Send(new GetAppointmentsByDTTableIdQuery() { DayttId = dttId });

                if (dttDto.ConsultDuration != null && dttDto.StartWorkTime != null && dttDto.TotalTicketQty != null
                    && (apmList != null || apmList?.Count != 0) && customerData != null)
                {
                    AppointmentIndexCreateModel model = new();
                    model.DayTimeTable = dttDto;
                    model.User = usr;
                    model.User.CustomerDataId = customerData.Id;
                    model.DoctorInfo = doctInfo;
                    var newApms = new List<AppointmentDto>();

                    for (int i = 0; i < dttDto.TotalTicketQty; i++)
                    {
                        var startTime = dttDto.StartWorkTime!.Value;
                        int diff = (int)(i * dttDto.ConsultDuration!);
                        startTime = startTime.AddMinutes(diff);
                        if (apmList!.Any(x => x.StartTime == startTime) || startTime < DateTime.Now)
                            continue;
                        else
                        {
                            var apm = new AppointmentDto()
                            {
                                StartTime = startTime,
                                EndTime = startTime.AddMinutes((double)dttDto.ConsultDuration),
                                DayTimeTableId = dttId,
                                CustomerDataId = customerData?.Id
                            };
                            newApms.Add(apm);
                        }
                    }

                    model.Appointments = newApms;
                    return Ok(model);
                }
                else
                    return NoContent();

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        /// Create Appointment
        /// </summary>
        ///  string? dttid, string? cdid, string? stime
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> Create([FromQuery] AppointmentRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Dttid) && string.IsNullOrEmpty(model.Cdid) && string.IsNullOrEmpty(model.Stime))
                return BadRequest("DayTimeTable/CustomerData Id or time is null");
            var resDttId = Guid.TryParse(model.Dttid, out var dttId);
            var resCdId = Guid.TryParse(model.Cdid, out var cdId);
            var resSTime = DateTime.TryParse(model.Stime, out DateTime startTime);
            if (!resCdId || !resSTime || !resDttId)
                return BadRequest("DayTimeTable/CustomerData Id or time is incorrect");
            try
            {
                var customerData = await _mediator.Send(new GetCDataByIdQuery(){CustomerDataId = cdId});
                var dttData = await _mediator.Send(new GetDayTimeTableByIdQuery() { DttId = dttId });

                if (customerData == null || dttData == null)
                    return NotFound("DayTimeTable/CustomerData is not found");

                var doctInfo = await _mediator.Send(new GetDoctorInfoByDocDataIdQuery() {DoctorDataId = dttData.DoctorDataId});
                if (doctInfo == null)
                    return NotFound("Doctor info is not found");
                var usr = await _modelBuilder.BuildUserDtoById(HttpContext, flag: 2);
                if (usr == null)
                    return NotFound("User is not found");

                AppointmentDto apmDto = new()
                {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    CustomerDataId = cdId,
                    DayTimeTableId = dttId,
                    StartTime = startTime,
                    EndTime = startTime.AddMinutes((double)dttData?.ConsultDuration!)
                };

                var res = await _mediator.Send(new AddApmCommand(){Apm = apmDto});

                AppointmentCreateModel newModel = new()
                {
                    DayTimeTableId = dttId,
                    Result = res,
                    Appointment = apmDto,
                    DoctorInfo = doctInfo,
                    User = usr
                };

                return Ok(newModel);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model MyApmsRequestModel"></param>
        /// string name, string speciality, string date, string sysInfo = "",
        /// SortState sortOrder = SortState.DateDesc
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> MyAppointments([FromQuery] MyApmsRequestModel model)
        {

            try
            {
                var usr = await _modelBuilder.BuildUserDtoById(HttpContext, flag: 2);
                if (usr == null)
                    return NotFound();
                var list = await _mediator.Send(new GetAppointmentsByUserIdQuery() { UsrId= usr.Id });
                if (list != null && list.Any())
                {
                    list = _adminSortFilter.AppointmentFilter(list, model.Speciality, model.Name,model. Date);
                    list = _adminSortFilter.AppointmentSort(list, model.SortOrder);
                    
                }

                string link = Request.Path.Value + Request.QueryString.Value;
                link = link.Replace("&", "*");
                //ViewData["Reflink"] = link;

                var listInfo = list?.Select(x => _mapper.Map<AppointmentInfo>
                                           ((x, x?.DayTimeTable?.DoctorData, x?.CustomerData ?? new CustomerDataDto())));

                if (!string.IsNullOrEmpty(model.SysInfo) && model.SysInfo.Equals("ok"))
                {
                    model.SysInfo = "Appointment was successfully deleted";
                }
                if (!string.IsNullOrEmpty(model.SysInfo) && model.SysInfo.Equals("no"))
                {
                    model.SysInfo = "Appointment was not deleted";
                }

                AppointmentIndexViewModel viewModel = new(listInfo, model.SysInfo, link,
                    new FilterAppointViewModel(model.Speciality, model.Name, model.Date, ""),
                    new SortViewModel(model.SortOrder));

                return Ok(viewModel);
            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        /// Delete Appointment
        /// </summary>
        /// <param name="model AppointmentDeleteRequest"></param>
        /// <returns></returns>
        /// string? id, string? reflink = ""
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> Delete([FromQuery]AppointmentDeleteRequest model)
        {
            string sysInfo = string.Empty;
            string redirect = string.Empty;
            if (model.Id == null)
                return BadRequest("Appointment Id is null");
            var resId = Guid.TryParse(model.Id, out Guid apmId);
            if (!resId)
                return BadRequest("Appointment Id is incorrect");
            try
            {
                var resDel = await _mediator.Send(new RemoveApmByIdCommand() { ApmId = apmId });
                if (resDel > 0)
                    sysInfo = "ok";
                else
                    sysInfo = "no";

                if (!string.IsNullOrEmpty(model.Reflink))
                    model.Reflink = model.Reflink.Replace("*", "&");
                if (!string.IsNullOrEmpty(model.Reflink) && model.Reflink.Contains('?'))
                    redirect = string.Join("", model.Reflink, "&sysInfo=", sysInfo);
                if (!string.IsNullOrEmpty(model.Reflink) && model.Reflink.Contains('?') == false)
                    redirect = string.Join("", model.Reflink, "?sysInfo=", sysInfo);

                return Redirect(redirect);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }


        /// <summary>
        /// Appointment ViewIndex
        /// </summary>
        /// <param name="model AppointmentIndexRequest"></param>
        /// <returns></returns>
        /// string? id (DTTID), string name, string birhtdate,
        /// string sysInfo = "", SortState sortOrder = SortState.DateAsc
        [HttpGet]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> ViewIndex([FromQuery]AppointmentIndexRequest model)
        {
            if (model.Id == null)
                return BadRequest("Daytimetable Id is null");
            var resId = Guid.TryParse(model.Id, out Guid dttId);
            if (!resId)
                return BadRequest("Daytimetable Id is incorrect");

            IEnumerable<AppointmentDto>? apmList = await _mediator.Send(new GetAppointmentsByDTTableIdQuery() { DayttId = dttId });
            if (apmList == null)
                return NotFound("Appointment list is not found");

            var dttDto = await _mediator.Send(new GetDayTimeTableByIdQuery() { DttId = dttId });
            if (dttDto == null || dttDto.DoctorDataId == null)
                return NotFound("Daytimetable is not found");

            string link = Request.Path.Value + Request.QueryString.Value;
            link = link.Replace("&", "*");
            //ViewData["Reflink"] = link;
            if (!string.IsNullOrEmpty(model.SysInfo) && model.SysInfo.Equals("ok"))
            {
                model.SysInfo = "Appointment was successfully deleted";
            }
            if (!string.IsNullOrEmpty(model.SysInfo) && model.SysInfo.Equals("no"))
            {
                model.SysInfo = "Appointment was not deleted";
            }

            try
            {
                if (apmList != null && apmList.Any())
                {
                    apmList = _adminSortFilter.AppointmentCustomerFilter(apmList, model.Name, model.Birthdate);
                    apmList = _adminSortFilter.AppointmentCustomerSort(apmList, model.SortOrder);  
                }
                var apmInfo =  apmList?.Select(x => _mapper.Map<AppointmentInfo>
                                              ((x, x?.DayTimeTable?.DoctorData ?? new DoctorDataDto(), x?.CustomerData)))
                                              .ToList();  
                if (apmInfo != null)
                {
                    for (int i = 0; i < apmInfo.Count(); i++)
                        apmInfo[i].DoctorDataId = dttDto.DoctorDataId;
                }

                AppointmentIndexViewModel viewModel = new(apmInfo, model.SysInfo, link,
                   new FilterAppointViewModel("", model.Name, "", model.Birthdate),
                   new SortViewModel(model.SortOrder));

                viewModel.DayTimeTableId = dttId;
                viewModel.DoctorDataId = (Guid)dttDto.DoctorDataId;
                return Ok(viewModel);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }


    }
}
