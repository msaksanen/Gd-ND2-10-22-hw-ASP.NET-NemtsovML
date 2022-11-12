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
using MedContactBusiness.ServicesImplementations;
using System.Security.Claims;
using MedContactApp.AdminPanelHelpers;
using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using Microsoft.EntityFrameworkCore;

namespace MedContactApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly EmailChecker _emailChecker;
        private int _pageSize = 7;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;
        private readonly AdminSortFilter _adminSortFilter;
        private readonly IDoctorDataService _doctorDataService;

        public DoctorController (IUserService userService, IConfiguration configuration,
            IMapper mapper, EmailChecker emailChecker, IRoleService roleService, 
            AdminSortFilter adminSortFilter, IDoctorDataService doctorDataService)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _emailChecker = emailChecker;
            _roleService = roleService;
            _adminSortFilter = adminSortFilter;
            _doctorDataService = doctorDataService; 
        }

        [HttpGet]
        public IActionResult DoctorMenu()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {
                var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                if (roles != null && roles.Any(r => r.Equals("Doctor")))
                    return View();
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string name, string surname,
              string speciality, int page = 1, SortState sortOrder = SortState.SpecialityAsc)
        {

            try
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                IQueryable<DoctorData> dDatas = _doctorDataService
                                                    .GetDoctorData()
                                                    .Include(d => d.Speciality)
                                                    //.Include(d => d.DayTimeTables)
                                                    .Include(d => d.User)
                                                    .ThenInclude(u => u!.Roles);

                dDatas = dDatas.Where(d => d.SpecialityId != null && d.IsBlocked != true && d.ForDeletion != true);
                dDatas = _adminSortFilter.DoctorDataFilter(dDatas, "", name, surname, speciality);
                dDatas = _adminSortFilter.DoctorDataSort(dDatas, sortOrder);

                var count = await dDatas.CountAsync();
                var doctorDatas = await dDatas.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                var items = doctorDatas.Select(dd => _mapper.Map<DoctorFullDataDto>((dd, dd.User, dd.Speciality)));

                string pageRoute = @"/doctor/index?page=";
                string processOptions = $"&name={name}&speciality={speciality}&sortorder={sortOrder}";
               
                string link = Request.Path.Value + Request.QueryString.Value;
                link = link.Replace("&", "*");
                ViewData["Reflink"] = link;

                DoctDataIndexViewModel viewModel = new(
                    items, processOptions,link,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterSpecViewModel(null,null, name, "", speciality),
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
                    //var doctorRole = await _roleService.GetRoleByNameAsync("Doctor"); 
                    var doctorDto = _mapper.Map<UserDto>(model);
                    if (doctorDto != null) // && doctorRole != null)
                    {
                        var result = await _userService.CreateUserWithRoleAsync(doctorDto, "Doctor");
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
                    var dto = _mapper.Map<UserDto>(model);

                    var sourceDto = await _userService.GetUserByIdAsync(dto.Id);

                    //should be sure that dto property naming is the same with entity property naming
                    //var patchList = new List<PatchModel>();
                    //Type myType = typeof(UserDto);
                    //var propList = myType?.GetProperties();
                    //if (propList!=null)
                    //{
                    //    foreach (var prop in propList)
                    //    {
                    //        var propName = myType?.GetProperty($"{prop.Name}");
                    //        var propValueModel = propName?.GetValue(dto);
                    //        var propValueSource = propName?.GetValue(sourceDto);
                    //        if (propValueSource != propValueModel && propValueModel != null)
                    //        {
                    //            PatchModel patchModel = new() { PropertyName = prop.Name, PropertyValue = propValueModel };
                    //            patchList.Add(patchModel);
                    //        }
                    //    }
                    //}
                    PatchMaker<UserDto> patchMaker = new PatchMaker<UserDto>();
                    var patchList = patchMaker.Make(dto, sourceDto);
                    await _userService.PatchAsync(dto.Id, patchList);
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

        [AcceptVerbs("Get", "Post")]
        public   async Task<IActionResult> CheckEmail(string email)
        {
            return Json(await _emailChecker.CheckEmail(email.ToLower(),HttpContext));
        }
        private async Task<UserDto?> GetDoctorDtoByIdAsync(string? id)
        {
            var result = Guid.TryParse(id, out Guid guid_id);

            if (result)
            {
                var doctor = await _userService.GetUserByIdAsync(guid_id);
                var doctorDto = _mapper.Map<UserDto>(doctor);
                return doctorDto;
            }
            return null;
        }

    }
}
