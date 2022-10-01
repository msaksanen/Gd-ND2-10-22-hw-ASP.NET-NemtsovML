//using Microsoft.AspNetCore.Mvc;
//using MedContactCore;
//using MedContactCore.Abstractions;
//using MedContactCore.DataTransferObjects;
//using MedContactApp.Models;
//using AutoMapper;
//using Serilog;
//using System.ComponentModel.Design;

//namespace MedContactApp.Controllers
//{
//    public class DoctorController : Controller
//    {
//        private readonly ICustomerService _customerService;
//        private readonly IMapper _mapper;
//        private int _pageSize = 5;

//        public DoctorController(ICustomerService customerService,
//            IMapper mapper)
//        {
//            _customerService = customerService;
//            _mapper = mapper;
//        }
//        public async Task<IActionResult> Index(int page)
//        {
//            try
//            {
//                var customers = await _customerService
//                    .GetCustomersByPageNumberAndPageSizeAsync(page, _pageSize);

//                if (customers.Any())
//                {
//                    return View(customers);
//                }
//                else
//                {
//                    throw new ArgumentException(nameof(page));
//                }
//            }
//            catch (Exception e)
//            {
//                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
//                return BadRequest();
//            }
//        }

//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create (CustomerModel model)
//        {
//            if (ModelState.IsValid)
//            {
//               // var userRoleId = await _roleService.GetRoleIdByNameAsync("User");
//                var customerDto = _mapper.Map<CustomerDto>(model);
//                if (customerDto != null) // && userRoleId != null
//                {
//                   //userDto.RoleId = userRoleId.Value;
//                    var result = await _customerService.CreateCustomerAsync(customerDto);
//                    if (result > 0)
//                    {
//                        //await Authenticate(model.Email);
//                        return RedirectToAction("Index", "Home");
//                    }
//                }
//            }
//            return View(model);
//        }

//    }
//}
