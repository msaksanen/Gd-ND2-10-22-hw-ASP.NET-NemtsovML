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

namespace MedContactApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IBaseUserService<DoctorDto> _doctorService;
        private readonly IMapper _mapper;
        private int _pageSize = 7;
        private readonly IConfiguration _configuration;

        public DoctorController (IBaseUserService<DoctorDto> doctorService, IConfiguration configuration,
            IMapper mapper)
        {
            _doctorService = doctorService;
            _mapper = mapper;
            _configuration = configuration;
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

    }
}
