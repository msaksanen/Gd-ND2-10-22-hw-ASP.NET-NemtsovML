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
using MedContactApp.Helpers;

namespace MedContactApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private int _pageSize = 7;
        private readonly IConfiguration _configuration;
        private readonly EmailChecker _emailChecker;
        private readonly IRoleService _roleService;
       // private readonly IRoleAllUserService<CustomerDto> _roleAllUserService;

        public CustomerController (IUserService userService, IConfiguration configuration,
            IMapper mapper, IRoleService roleService,
            EmailChecker emailChecker)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _roleService = roleService;
            //_roleAllUserService = roleAllUserService;
            _emailChecker = emailChecker;
        }
        public async Task<IActionResult> Index(int page)
        {
            try
            {
               bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
               if (result) _pageSize=pageSize;

               var customers = await _userService
                    .GetUsersByPageNumberAndPageSizeAsync(page, _pageSize);

                if (customers.Any())
                {
                    double count= await _userService.GetUserEntitiesCountAsync();
                    double pageCount = Math.Ceiling(count/ _pageSize);
                    ViewBag.pageCount = pageCount;
                    ViewBag.currentPage = page;
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
                    //var customerRole = await _roleService.GetRoleByNameAsync("Customer");    
                    var customerDto = _mapper.Map<UserDto>(model);
                    if (customerDto != null) //&& customerRole!= null
                    {
                        var result = await _userService.CreateUserWithRoleAsync(customerDto, "Customer");
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

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            return Json(await _emailChecker.CheckEmail(email.ToLower()));
        }

    }
}
