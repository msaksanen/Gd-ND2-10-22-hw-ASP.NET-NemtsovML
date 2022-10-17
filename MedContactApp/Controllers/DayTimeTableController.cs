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


namespace MedContactApp.Controllers
{
    public class DayTimeTableController : Controller
    {
        private readonly IDayTimeTableService _dayTimeTableService;
        private readonly IMapper _mapper;
        private readonly IDoctorDataService _doctorDataService;
        private readonly IConfiguration _configuration;
        private int _pageSize = 7;

        public DayTimeTableController(IDayTimeTableService dayTimeTableService, IConfiguration configuration,
            IMapper mapper, IDoctorDataService doctorDataService)
        {
            _dayTimeTableService = dayTimeTableService;
            _mapper = mapper;
            _configuration = configuration;
            _doctorDataService = doctorDataService;
        }

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
        public IActionResult Create (string? id)
        {
            var result = Guid.TryParse(id, out Guid guid_id);
            if (result)
            {
                var model = new DayTimeTableModel();
                model.DoctorDataId = guid_id;
                model.Date = DateTime.Now;
                model.StartWorkTime = DateTime.Today.AddHours(8);
                model.FinishWorkTime = DateTime.Today.AddHours(20);
                return View(model);
            }

            ModelState.AddModelError("CustomError", $"Id {id} is invalid.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Create (DayTimeTableModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = Guid.NewGuid();
                    model.CreationDate = DateTime.Now;
                    var dto = _mapper.Map<DayTimeTableDto>(model);
                    if (dto != null)
                    {
                        var result = await _dayTimeTableService.CreateDayTimeTableAsync(dto);
                        if (result > 0)
                        {
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
