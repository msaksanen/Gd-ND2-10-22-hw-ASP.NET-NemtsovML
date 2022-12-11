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
using System.Text;

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
        private int _termDays = 1;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly AdminSortFilter _adminSortFilter;
        private readonly EmailSender _emailSender;
        private readonly IUserService _userService;


        public AppointmentController(IDayTimeTableService dayTimeTableService, IConfiguration configuration,
            IMapper mapper, IDoctorDataService doctorDataService, ModelUserBuilder modelBuilder,
            IAppointmentService appointmentService, ICustomerDataService customerDataService, 
            AdminSortFilter adminSortFilter, IUserService userService, EmailSender emailSender)
        {
            _dayTimeTableService = dayTimeTableService;
            _mapper = mapper;
            _configuration = configuration;
            _doctorDataService = doctorDataService;
            _modelBuilder = modelBuilder;
            _appointmentService = appointmentService;
            _customerDataService = customerDataService;
            _adminSortFilter = adminSortFilter;
            _userService = userService; 
            _emailSender = emailSender;
        }

        [HttpGet]
        [Authorize (Policy = "FullBlocked")]
        public async Task<IActionResult> CreateIndex(string? id, string? uid)
        {
            if (string.IsNullOrEmpty(id))
                //return BadRequest();
                  return new  BadRequestObjectResult("DayTimeTable Id is null");
            var res = Guid.TryParse(id, out Guid dttId);
            if (!res)
                //return BadRequest();
                return new BadRequestObjectResult("DayTimeTable Id is incorrect");
            UserDto? usr = new();
            try
            {
                var dttDto = await _dayTimeTableService.GetDayTimeTableByIdAsync(dttId);
                if (dttDto == null)
                    //return NotFound();
                      return NotFound("DayTimeTable is not found");
                var doctInfo = await _doctorDataService.GetDoctorInfoById(dttDto.DoctorDataId);
                if (doctInfo == null)
                    //return NotFound();
                      return NotFound("Doctor info is not found");
                if (!string.IsNullOrEmpty(uid) && Guid.TryParse(uid, out Guid userId))
                {
                  usr = await _userService.GetUserByIdAsync(userId);
                }
                else
                {
                    usr = await _modelBuilder.BuildUserById(HttpContext, flag: 2);
                }
                if (usr == null)
                    //return NotFound();
                     return NotFound("User is not found");

                var customerData = await _customerDataService.GetOrCreateByUserIdAsync(usr.Id);

                var apmList = await _appointmentService.GetAppointmentsByDTTableIdAsync(dttId);

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
            int flag = 0;
            if (string.IsNullOrEmpty(dttid) && string.IsNullOrEmpty(cdid) && string.IsNullOrEmpty(stime))
                //return BadRequest();
                return new BadRequestObjectResult("DayTimeTable/CustomerData Id or time is null");
            var resDttId = Guid.TryParse(dttid, out var dttId);
            var resCdId = Guid.TryParse(cdid, out var cdId);
            var resSTime = DateTime.TryParse(stime, out DateTime startTime);
            if (!resCdId || !resSTime || !resDttId)
               // return BadRequest();
                  return new BadRequestObjectResult("DayTimeTable/CustomerData Id or time is incorrect");
            try
            {
                var customerData = await _customerDataService.GetByIdAsync(cdId);
                var dttData = await _dayTimeTableService.GetDayTimeTableByIdAsync(dttId);

                if (customerData == null || dttData == null)
                   //return NotFound();
                     return NotFound("DayTimeTable/CustomerData is not found");

                var doctInfo = await _doctorDataService.GetDoctorInfoById(dttData.DoctorDataId);
                if (doctInfo == null)
                    //return NotFound();
                      return NotFound("Doctor info is not found");
                //var usr = await _modelBuilder.BuildUserById(HttpContext, flag: 2);
                var usr = await _userService.GetUserByIdAsync((Guid)customerData.UserId!);
                if (usr == null)
                    //return NotFound();
                     return NotFound("User is not found"); 

                if (User.IsInRole("Doctor"))
                {
                    flag = 1;
                }

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
                    User = usr,
                    Flag = flag
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
                viewModel.User = usr;
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
                //return BadRequest();
                  return new BadRequestObjectResult("Appointment Id is null");
            var resId = Guid.TryParse(id, out Guid apmId);
            if (!resId)
                //return BadRequest();
                  return new BadRequestObjectResult("Appointment Id is incorrect");
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
                //return BadRequest();
                return new BadRequestObjectResult("Daytimetable Id is null");
            var resId = Guid.TryParse(id, out Guid dttId);
            if (!resId)
                //return BadRequest();
                return new BadRequestObjectResult("Daytimetable Id is incorrect");

            IEnumerable<AppointmentDto>? apmList = await _appointmentService.GetAppointmentsByDTTableIdAsync(dttId);
            if (apmList == null)
                //return NotFound();
                return NotFound("Appointment list is not found");

            var dttDto = await _dayTimeTableService.GetDayTimeTableByIdAsync(dttId);
            if (dttDto == null || dttDto.DoctorDataId==null)
                //return NotFound();
                return NotFound("Daytimetable is not found");

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

        [HttpGet]
        [Authorize(Roles = "Doctor", Policy = "FullBlocked")]
        public async Task<IActionResult> PatientViewIndex(string? dataid, string name, string birhtdate,
           string sysInfo = "", SortState sortOrder = SortState.DateAsc)
        {
            try
            {
              if (dataid == null)
                //return BadRequest();
                return new BadRequestObjectResult("DoctorData Id is null");
              var resId = Guid.TryParse(dataid, out Guid doctId);
              if (!resId)
                //return BadRequest();
                return new BadRequestObjectResult("DoctorData Id is incorrect");
              var doctorInfo = await _doctorDataService.GetDoctorInfoById(doctId);
              if (doctorInfo == null)
                //return NotFound();
                return NotFound("Doctor info is not found");

              var patList = await _appointmentService.GetPatientsByDoctorDataId(doctId);
              if (patList == null || !patList.Any())
                //return NotFound();
                return NotFound("Patient list is not found");

           
                var id = patList[0]!.CustomerData!.UserId;
                List<AppointmentDto> distinctLst = new();
                foreach (var item in patList)
                {
                    if (item.CustomerData!=null && item.CustomerData.UserId == id)
                        continue;
                    else if(item.CustomerData != null)
                    {
                       distinctLst.Add(item);
                      id = item.CustomerData.UserId;
                    }  
                }

                var lst= distinctLst.AsEnumerable();

                string link = Request.Path.Value + Request.QueryString.Value;
                link = link.Replace("&", "*");
                ViewData["Reflink"] = link;
         
           
                if (patList != null && patList.Any())
                {
                    lst = _adminSortFilter.AppointmentCustomerFilter(lst, name, birhtdate);
                    lst = _adminSortFilter.AppointmentCustomerSort(lst, sortOrder);
                }

                PatientIndexViewModel viewModel = new(lst, sysInfo, doctorInfo,
                   new FilterAppointViewModel("", name, "", birhtdate),
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
        [Authorize(Roles = "Admin", Policy = "FullBlocked")]

        public async Task<IActionResult> SendAppointmentReminders()
        {
            bool result = int.TryParse(_configuration["EmailSendMode:DaysTerm"], out var term);
            if (result) _termDays = term;
            StringBuilder sb = new StringBuilder();
            var mailList = await _appointmentService.GetAppointmentMailListAsync(_termDays);
            if (mailList!=null && mailList.Any())
            {
                foreach (var item in mailList)
                {
                    string mailbody = GenerateMailBody(item);
                    string subj= $"MedContact: Doctor Appointment - {DateTime.Now.AddDays(term).ToShortDateString()}";
                    var res = await _emailSender.SendEmailAsync(item?.CustomerEmail, subj, mailbody);
                    sb.AppendLine("============================");
                    sb.AppendLine($"Appointment Id ({item?.Id})");
                    sb.AppendLine($"Result: {res.Name}");
                    sb.AppendLine("============================");

                    if (res.IntResult==0)
                        Log.Error($"EmailSender: {res.Name}");
                   else
                        Log.Information($"EmailSender: {res.Name}");
                }
            }

            return new ObjectResult(sb.ToString());
        }

        private string GenerateMailBody(AppointmentInfo item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Appointment № {item.Id}");
            string temp = string.Join(" ", "TIME:", item?.StartTime?.ToShortDateString(),
                          "(", item?.StartTime?.ToShortTimeString(),"-", item?.EndTime?.ToShortTimeString(),")");
            sb.AppendLine(temp);
            sb.AppendLine("============================");
            sb.AppendLine($"Customer: {item?.CustomerName} {item?.CustomerMidname} {item?.CustomerSurname}");
            sb.AppendLine($"Customer email: {item?.CustomerEmail}");
            sb.AppendLine("============================");
            sb.AppendLine($"Doctor: {item?.DoctorName} {item?.DoctorMidname} {item?.DoctorSurname}");
            sb.AppendLine($"Doctor speciality: {item?.DoctorSpeciality}");
            sb.AppendLine("============================");
            sb.AppendLine($"Creation Time: {item?.CreationDate}");
            sb.AppendLine("============================");

            return sb.ToString();   
        }

    }
}
