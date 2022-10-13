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
using Microsoft.Extensions.Configuration;
using MedContactDb.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MedContactBusiness.ServicesImplementations;

namespace MedContactApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBaseUserService<CustomerDto> _customerService;
        private readonly IBaseUserService<DoctorDto> _doctorService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IRoleAllUserService<CustomerDto> _cusRoleAllUserService;
        private readonly IRoleAllUserService<DoctorDto> _docRoleAllUserService;

        public AccountController(IBaseUserService<CustomerDto> customerService,
            IMapper mapper, IRoleService roleService, IRoleAllUserService<CustomerDto> cusRoleAllUserService,
            IBaseUserService<DoctorDto> doctorService, IRoleAllUserService<DoctorDto> docRoleAllUserService)
        {
            _customerService = customerService;
            _mapper = mapper;
            _roleService = roleService;
            _cusRoleAllUserService = cusRoleAllUserService;
            _docRoleAllUserService = docRoleAllUserService;
            _doctorService = doctorService;
        }

        [HttpGet]
        public IActionResult RegCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegCustomer(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newres = await RegBaseUserAsync<CustomerModel, CustomerDto>(model);
                    if (newres > 0)
                    {
                        //await Authenticate(model.Email);
                        return RedirectToAction("Index", "Home");
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
        public IActionResult RegDoctor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegDoctor(DoctorModel model)
        {
            if (ModelState.IsValid)
            {
                model.IsBlocked = true;
                try
                {
                        var newres = await RegBaseUserAsync<DoctorModel, DoctorDto>(model);
                        if (newres > 0)
                        {
                            //await Authenticate(model.Email);
                            return RedirectToAction("Index", "Home");
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

        private async Task<int> RegBaseUserAsync<BM, DTO>(BM bModel) 
                       where BM : BaseUserModel
                       where DTO : BaseUserDto
        {
            int result = -1;
            string roleName =bModel!.RoleName!;

            if (!String.IsNullOrEmpty(roleName))
            {
                var role = await _roleService.GetRoleByNameAsync(roleName);
                var dto = _mapper.Map<DTO>(bModel);
                if (dto != null && role != null)
                {
                    dto.RoleName = role.Name;
                    dto.RoleId = role.Id;
                    if (typeof(DTO).Equals(typeof(CustomerDto)) && dto!=null)
                    {
                        CustomerDto? cDto = dto as CustomerDto;
                        result = await _cusRoleAllUserService.RegisterWithRoleAsync(cDto!);
                    }
                    if (typeof(DTO).Equals(typeof(DoctorDto)) && dto != null)
                    {
                        DoctorDto? dDto = dto as DoctorDto;
                        result = await _docRoleAllUserService.RegisterWithRoleAsync(dDto!);
                    }
                }

            }
            return result;
        }
    }
}
