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
    public class CustomerController : Controller
    {
        private readonly IBaseUserService<CustomerDto> _customerService;
        private readonly IMapper _mapper;
        private int _pageSize = 7;
        private readonly IConfiguration _configuration;

        public CustomerController (IBaseUserService<CustomerDto> customerService, IConfiguration configuration,
            IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(int page)
        {
            try
            {
               bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
               if (result) _pageSize=pageSize;

               var customers = await _customerService
                    .GetBaseUsersByPageNumberAndPageSizeAsync(page, _pageSize);

                if (customers.Any())
                {
                    var count= await _customerService.GetBaseUserEntitiesCountAsync();
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
        public async Task<IActionResult> Create (CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    // var userRoleId = await _roleService.GetRoleIdByNameAsync("User");
                    model.Role = "Customer";
                    var customerDto = _mapper.Map<CustomerDto>(model);
                    if (customerDto != null) // && userRoleId != null
                    {
                        //userDto.RoleId = userRoleId.Value;
                        var result = await _customerService.CreateBaseUserAsync(customerDto);
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
