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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
        private readonly IRoleAllUserService<UserDto> _usrRoleAllUserService;

        public AccountController(IBaseUserService<CustomerDto> customerService,
            IMapper mapper, IRoleService roleService, IRoleAllUserService<CustomerDto> cusRoleAllUserService,
            IBaseUserService<DoctorDto> doctorService, IRoleAllUserService<DoctorDto> docRoleAllUserService, 
            IRoleAllUserService<UserDto> usrRoleAllUserService)
        {
            _customerService = customerService;
            _doctorService = doctorService;
            _mapper = mapper;
            _roleService = roleService;
            _cusRoleAllUserService = cusRoleAllUserService;
            _docRoleAllUserService = docRoleAllUserService;
            _usrRoleAllUserService = usrRoleAllUserService; 
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

        private async Task Authenticate<DTO> (string email)
                      where DTO : BaseUserDto
        {
            DTO? dto = (DTO)new BaseUserDto();
            if (typeof(DTO).Equals(typeof(CustomerDto)))
            {
                CustomerDto? cdto = new CustomerDto();
                cdto= await _customerService.GetBaseUserByEmailAsync(email);
                dto = cdto as DTO;
            }
           
            if (dto!=null && dto.Email != null && dto.RoleName!=null)
            {
                var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, dto.RoleName)
            };

                var identity = new ClaimsIdentity(claims,
                    "ApplicationCookie",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType
                );

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
           
        }

        private async Task<int> RegBaseUserAsync<BM,DTO> (BM bModel) 
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
                    if (typeof(DTO).Equals(typeof(UserDto)) && dto != null)
                    {
                        UserDto? uDto = dto as UserDto;
                        result = await _usrRoleAllUserService.RegisterWithRoleAsync(uDto!);
                    }
                }

            }
            return result;
        }
    }
}
