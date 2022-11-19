using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactApp.Helpers;
using MedContactApp.Models;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using Serilog;
using System.Data;
using System.Security.Claims;
using MedContactDb.Entities;
using System.Linq;
using MedContactApp.AdminPanelHelpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace MedContactApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IDayTimeTableService _dayTimeTableService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly IDoctorDataService _doctorDataService;
        private readonly ICustomerDataService _customerDataService;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly AdminSortFilter _adminSortFilter;


        public AppointmentController(IDayTimeTableService dayTimeTableService, IConfiguration configuration,
            IMapper mapper, IDoctorDataService doctorDataService, ModelUserBuilder modelBuilder,
            IAppointmentService appointmentService, ICustomerDataService customerDataService, AdminSortFilter adminSortFilter)
        {
            _dayTimeTableService = dayTimeTableService;
            _mapper = mapper;
            _configuration = configuration;
            _doctorDataService = doctorDataService;
            _modelBuilder = modelBuilder;
            _appointmentService = appointmentService;
            _customerDataService = customerDataService;
            _adminSortFilter = adminSortFilter;
        }

        [HttpGet]
        [Authorize (Policy = "FullBlocked")]
        public async Task<IActionResult> CreateIndex(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            var res = Guid.TryParse(id, out Guid dttId);
            if (!res)
                return BadRequest();
            try
            {
                var dttDto = await _dayTimeTableService.GetDayTimeTableByIdAsync(dttId);
                if (dttDto == null)
                    return NotFound();
                var doctInfo = await _doctorDataService.GetDoctorInfoById(dttDto.DoctorDataId);
                if (doctInfo == null)
                    return NotFound();
                var usr = await _modelBuilder.BuildUserById(HttpContext, flag: 2);
                if (usr == null)
                    return NotFound();

                var customerData = await _customerDataService.GetOrCreateByUserIdAsync(usr.Id);

                var apmList = await _appointmentService.GetAppointmentsByDTTableIdAsync(dttId);

                if (dttDto.ConsultDuration != null && dttDto.StartWorkTime != null && dttDto.TotalTicketQty != null
                    && (apmList != null || apmList?.Count != 0) && customerData != null)
                {
                    AppointmentIndexCreateModel model = new();
                    model.DayTimeTable = dttDto;
                    model.User = usr;
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
                    return View(model);
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

        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> Create(string? dttid, string? cdid, string? stime)
        {
            if (string.IsNullOrEmpty(dttid) && string.IsNullOrEmpty(cdid) && string.IsNullOrEmpty(stime))
                return BadRequest();
            var resDttId = Guid.TryParse(dttid, out var dttId);
            var resCdId = Guid.TryParse(cdid, out var cdId);
            var resSTime = DateTime.TryParse(stime, out DateTime startTime);
            if (!resCdId || !resSTime || !resDttId)
                return BadRequest();
            try
            {
                var customerData = await _customerDataService.GetByIdAsync(cdId);
                var dttData = await _dayTimeTableService.GetDayTimeTableByIdAsync(dttId);

                if (customerData == null || dttData == null)
                    return NotFound();

                var doctInfo = await _doctorDataService.GetDoctorInfoById(dttData.DoctorDataId);
                if (doctInfo == null)
                    return NotFound();
                var usr = await _modelBuilder.BuildUserById(HttpContext, flag: 2);
                if (usr == null)
                    return NotFound();

                AppointmentDto apmDto = new()
                {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    CustomerDataId = cdId,
                    DayTimeTableId = dttId,
                    StartTime = startTime,
                    EndTime = startTime.AddMinutes((double)dttData?.ConsultDuration!)
                };

                var res = await _appointmentService.Add(apmDto);

                AppointmentCreateModel model = new()
                {
                    DayTimeTableId = dttId,
                    Result = res,
                    Appointment = apmDto,
                    DoctorInfo = doctInfo,
                    User = usr
                };

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
        public async Task<IActionResult> MyAppointments(string name, string speciality, string date, string sysInfo = "",
               SortState sortOrder = SortState.DateDesc)
        {

            try
            {
                var usr = await _modelBuilder.BuildUserById(HttpContext, flag: 2);
                if (usr == null)
                    return NotFound();
                var list = await _appointmentService.GetAppointmentsByUserId(usr.Id);
                if (list != null && list.Any())
                {
                    list = _adminSortFilter.AppointmentFilter(list, speciality, name, date);
                    list = _adminSortFilter.AppointmentSort(list, sortOrder);
                }

                string link = Request.Path.Value + Request.QueryString.Value;
                link = link.Replace("&", "*");
                ViewData["Reflink"] = link;

                if (!string.IsNullOrEmpty(sysInfo) && sysInfo.Equals("ok"))
                {
                    sysInfo = "Appointment was successfully deleted";
                }
                if (!string.IsNullOrEmpty(sysInfo) && sysInfo.Equals("no"))
                {
                    sysInfo = "Appointment was not deleted";
                }

                AppointmentIndexViewModel viewModel = new(list, sysInfo,
                    new FilterAppointViewModel(speciality, name, date,""),
                    new SortViewModel(sortOrder));

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
        public async Task<IActionResult> Delete(string? id, string? reflink = "")
        {
            string sysInfo = string.Empty;
            string redirect= string.Empty;
            if (id == null)
                return BadRequest();
            var resId = Guid.TryParse(id, out Guid apmId);
            if (!resId)
                return BadRequest();
            try
            {
                var resDel = await _appointmentService.RemoveById(apmId);
                if (resDel != null && resDel > 0)
                    sysInfo = "ok";

                else
                    sysInfo = "no";

                if (!string.IsNullOrEmpty(reflink))
                    reflink = reflink.Replace("*", "&");
                if (!string.IsNullOrEmpty(reflink) && reflink.Contains('?'))
                    redirect = string.Join("", reflink, "&sysInfo=", sysInfo);
                if (!string.IsNullOrEmpty(reflink) && reflink.Contains('?') == false)
                    redirect = string.Join("", reflink, "?sysInfo=", sysInfo);

                return Redirect(redirect);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> ViewIndex(string? id, string name, string birhtdate,
            string sysInfo = "", SortState sortOrder = SortState.DateAsc)
        {
            if (id == null)
                return BadRequest();
            var resId = Guid.TryParse(id, out Guid dttId);
            if (!resId)
                return BadRequest();

            IEnumerable<AppointmentDto>? apmList = await _appointmentService.GetAppointmentsByDTTableIdAsync(dttId);
            if (apmList == null)
                return NotFound();

            var dttDto = await _dayTimeTableService.GetDayTimeTableByIdAsync(dttId);
            if (dttDto == null || dttDto.DoctorDataId==null)
                return NotFound();

            string link = Request.Path.Value + Request.QueryString.Value;
            link = link.Replace("&", "*");
            ViewData["Reflink"] = link;
            if (!string.IsNullOrEmpty(sysInfo) && sysInfo.Equals("ok"))
            {
                sysInfo = "Appointment was successfully deleted";
            }
            if (!string.IsNullOrEmpty(sysInfo) && sysInfo.Equals("no"))
            {
                sysInfo = "Appointment was not deleted";
            }

            try 
            {
                if (apmList != null && apmList.Any())
                {
                    apmList = _adminSortFilter.AppointmentCustomerFilter(apmList, name, birhtdate);
                    apmList = _adminSortFilter.AppointmentCustomerSort(apmList, sortOrder);
                }

                AppointmentIndexViewModel viewModel = new(apmList, sysInfo,
                   new FilterAppointViewModel("", name, "", birhtdate),
                   new SortViewModel(sortOrder));
                viewModel.DayTimeTableId = dttId;
                viewModel.DoctorDataId = (Guid)dttDto.DoctorDataId;
                return View(viewModel);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }
    }
}
