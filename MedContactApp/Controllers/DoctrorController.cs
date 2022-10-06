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
using System;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using NuGet.Packaging;
using MedContactApp.Helpers;


namespace MedContactApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IBaseUserService<DoctorDto> _doctorService;
        private readonly IMapper _mapper;
        private readonly EmailChecker<DoctorDto> _emailChecker;
        private int _pageSize = 7;
        private readonly IConfiguration _configuration;

        public DoctorController (IBaseUserService<DoctorDto> doctorService, IConfiguration configuration,
            IMapper mapper, EmailChecker<DoctorDto> emailChecker)
        {
            _doctorService = doctorService;
            _mapper = mapper;
            _configuration = configuration;
            _emailChecker = emailChecker;
        }
        public async Task<IActionResult> Index(int page)
        {
            try
            {
               bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
               if (result) _pageSize=pageSize;

               var customers = await _doctorService
                    .GetBaseUsersByPageNumberAndPageSizeAsync(page, _pageSize);

                if (customers.Any())
                {
                    var count= await _doctorService.GetBaseUserEntitiesCountAsync();
                    int pageCount = (int)Math.Ceiling((double)(count/ _pageSize))+1;
                    ViewBag.pageCount = pageCount;
                    return View(customers);
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create (DoctorModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    // var userRoleId = await _roleService.GetRoleIdByNameAsync("User");
                    model.Role = "Doctor";
                    var doctorDto = _mapper.Map<DoctorDto>(model);
                    if (doctorDto != null) // && userRoleId != null
                    {
                        //userDto.RoleId = userRoleId.Value;
                        var result = await _doctorService.CreateBaseUserAsync(doctorDto);
                        if (result > 0)
                        {
                            //await Authenticate(model.Email);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }               
            }
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
           var doctorDto = await GetDoctorDtoByIdAsync(id);
           if (doctorDto!=null) 
               return View(doctorDto);
     
           ModelState.AddModelError("CustomError", $"Doctor with id {id} is not found.");
               return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            var doctorDto = await GetDoctorDtoByIdAsync(id);
            if (doctorDto != null)
                return View(doctorDto);

            ModelState.AddModelError("CustomError", $"Doctor with id {id} is not found.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit (DoctorModel model)
        {
            try
            {
                if (model != null)
                {
                    var dto = _mapper.Map<DoctorDto>(model);

                    var sourceDto = await _doctorService.GetBaseUserByIdAsync(dto.Id);

                    //should be sure that dto property naming is the same with entity property naming
                    var patchList = new List<PatchModel>();
                    Type myType = typeof(DoctorDto);
                    var propList = myType?.GetProperties();
                    if (propList!=null)
                    {
                        foreach (var prop in propList)
                        {
                            var propName = myType?.GetProperty($"{prop.Name}");
                            var propValueModel = propName?.GetValue(dto);
                            var propValueSource = propName?.GetValue(sourceDto);
                            if (propValueSource != propValueModel && propValueModel != null)
                            {
                                PatchModel patchModel = new() { PropertyName = prop.Name, PropertyValue = propValueModel };
                                patchList.Add(patchModel);
                            }
                        }
                    }
                    await _doctorService.PatchAsync(dto.Id, patchList);
                    return RedirectToAction("Index","Doctor");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public  IActionResult DayTimeTable (string? id)
        {
            var result = Guid.TryParse(id, out Guid guid_id);
            if (result)
            {
                var model = new DayTimeTableModel();
                model.DoctorId = guid_id;
                model.Date= DateTime.Now;
                model.StartWorkTime = DateTime.Today.AddHours(8);
                model.FinishWorkTime = DateTime.Today.AddHours(20);
                return View(model);
            }

            ModelState.AddModelError("CustomError", $"Id {id} is invalid.");
            return RedirectToAction("Index", "Home");
        }

        [AcceptVerbs("Get", "Post")]
        public   async Task<IActionResult> CheckEmail(string email)
        {
            return Json(await _emailChecker.CheckEmail(email.ToLower()));
        }
        private async Task<DoctorDto?> GetDoctorDtoByIdAsync(string? id)
        {
            var result = Guid.TryParse(id, out Guid guid_id);

            if (result)
            {
                var doctor = await _doctorService.GetBaseUserByIdAsync(guid_id);
                var doctorDto = _mapper.Map<DoctorDto>(doctor);
                return doctorDto;
            }
            return null;
        }

    }
}
