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
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using MedContactApp.Helpers;
using Microsoft.CodeAnalysis.Differencing;
using MedContactApp.FilterSortHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore.Query;
using MedContactApp.FilterSortPageHelpers;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Build.Framework;
using System.Data;
using System.Xml.Linq;

namespace MedContactApp.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly IFileDataService _fileDataService;
        private readonly IDoctorDataService _doctorDataService;
        private readonly IWebHostEnvironment _appEnvironment;
        private int _pageSize = 7;

        public AdminPanelController(IUserService userService,
            IMapper mapper, IRoleService roleService, IConfiguration configuration, 
            ModelUserBuilder modelBuilder, IFileDataService fileDataService, IDoctorDataService doctorDataService, 
            IWebHostEnvironment appEnvironment)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
            _modelBuilder = modelBuilder;
            _fileDataService = fileDataService;
            _doctorDataService = doctorDataService;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public IActionResult AdminPanelMenu()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {
                var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                if (roles != null && roles.Any(r => r.Equals("Admin")))
                    return View();
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> UserIndex(string email, string name, string surname, string role, int page = 1,
               SortState sortOrder = SortState.EmailAsc)

        {

            var res = Guid.TryParse(role, out Guid roleId);
            if (!res)
                roleId = default;
            try
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                IQueryable<User> users = _userService.GetUsers().Include(u => u.Roles);
                var roles = _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToList();

                if (roleId != default)
                {
                    users = (from u in users
                             from r in u.Roles!
                             where r.Id.Equals(roleId)
                             select u);
                }

                users = UserFilter(users, email, name, surname);
                users = UserSort(users, sortOrder);

                var count = await users.CountAsync();
                var items = await users.Skip((page - 1) * pageSize).Take(pageSize)
                           .Select(user => _mapper.Map<UserDto>(user)).ToListAsync();

                string pageRoute = @"/adminpanel/userindex?page=";
                string processOptions = $"&email{email}&name={name}&roleid={roleId}&sortorder={sortOrder}";

                UserIndexViewModel viewModel = new(
                    items, processOptions,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterViewModel(roles, roleId, name, email),
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
        public async Task<IActionResult> DoctorDataIndex(string email, string name, string surname, string role, 
            string speciality, int page = 1, SortState sortOrder = SortState.EmailAsc)
        {

            var res = Guid.TryParse(role, out Guid roleId);
            if (!res)
                roleId = default;
            try
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                IQueryable<DoctorData> dDatas = _doctorDataService
                                                    .GetDoctorData()
                                                    .Include(d => d.Speciality)
                                                    .Include(d => d.DayTimeTables)
                                                    .Include(d => d.User)
                                                    .ThenInclude(u => u!.Roles);

                var roles = _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToList();

                if (roleId != default)
                {
                    dDatas = (from d in dDatas
                              from r in d.User!.Roles!
                              where r.Id.Equals(roleId)
                              select d);
                }

                dDatas = DoctorDataFilter(dDatas, email, name, surname, speciality);
                dDatas = DoctorDataSort(dDatas, sortOrder);

                var count = await dDatas.CountAsync();
                var listData = await dDatas.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                var items = listData.Select(dd => _mapper.Map<DoctorFullDataDto>((dd, dd.User, dd.Speciality))).ToList();
                //DoctorData, User, Speciality
                //  var docInfoList = dDataList.Select(t => _mapper.Map<DoctorInfo>((t.User, t.Speciality, t.ForDeletion))).ToList();

                string pageRoute = @"/adminpanel/doctordataindex?page=";
                string processOptions = $"&email{email}&name={name}&speciality={speciality}&roleid={roleId}&sortorder={sortOrder}";

                DoctDataIndexViewModel viewModel = new(
                    items, processOptions,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterSpecViewModel(roles, roleId, name, email, speciality),
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
        public async Task<IActionResult> ApplicationIndex(string email, string name, string surname, int page = 1,
             SortState sortOrder = SortState.EmailAsc)

        {
            try
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                IQueryable<User> users = _userService.GetUsers().Include(u => u.Roles);
                IQueryable<User> applicants = from u in users
                                              from r in u.Roles!
                                              where r.Name!.Equals("Applicant")
                                              select u;

                applicants = UserFilter(applicants, email, name, surname);
                applicants = UserSort(applicants, sortOrder);

                var count = await applicants.CountAsync();
                var items = await applicants.Skip((page - 1) * pageSize).Take(pageSize)
                           .Select(user => _mapper.Map<UserDto>(user)).ToListAsync();

                string pageRoute = @"/adminpanel/applicationindex?page=";
                string processOptions = $"&email={email}&name={name}&sortorder={sortOrder}";

                UserIndexApplViewModel viewModel = new(
                    items, processOptions,
                    new PageViewModel(count, page, pageSize, pageRoute),
                    new FilterNameViewModel(name, email),
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
        public async Task<IActionResult> ApplicantDetails(string? id, string? filedata="")
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var res = Guid.TryParse(id, out Guid Id);
            if (!res)
                return BadRequest();
            var usr = await _userService.GetUserByIdAsync(Id);
            if(usr == null)
                return NotFound();

            var model = _mapper.Map<ApplicantModel>(usr);

            if (!string.IsNullOrEmpty(filedata))
            {
                var resfile = Guid.TryParse(filedata, out Guid fdId);
                if (!resfile)
                    model.SystemInfo = $"<b>File data Id({filedata}) for deletion is incorrect.</b>";
                var removeResult = await _fileDataService.RemoveFileDataWithFileById(fdId, _appEnvironment.WebRootPath);

                switch(removeResult)
                {
                    case -2:
                        model.SystemInfo = $"<b>FileData with Id({filedata}) was not found <br/>File cannot be deleted</b>";
                        break;
                    case -1:
                        model.SystemInfo = $"<b>FileData with Id({filedata}) has incorrect path <br/>File cannot be deleted</b>";
                        break;
                    case 1:
                        model.SystemInfo = $"<b>FileData with Id({filedata}) was deleted <br/>The file was removed before</b>";
                        break;
                    case 2:
                        model.SystemInfo = $"<b>FileData with Id({filedata}) and its file were deleted</b>";
                        break;
                }
            }
            var fileList = await _fileDataService.FileDataTolistByUserId(Id);
            if (fileList!=null && fileList.Any())
                fileList= fileList.Where(x => x.Category != null && x.Category.Equals("Applicant")).ToList();
            model.fileDatas = fileList;

            var doctInfoList = await _doctorDataService.GetDoctorInfoByUserId(Id);
            if (doctInfoList != null && doctInfoList.Any())
                model.doctorInfos = doctInfoList;

            //ViewBag.WebRootPath = _appEnvironment.WebRootPath;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string? id, string? reflink="")
        {

            AdminUserEditModel emptyModel = new();
            var model = await BuildByIdAdmin(emptyModel, id);
            if (model == null)
                return NotFound();

           model.Reflink = reflink;

           return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserDetails(AdminUserEditModel model)
        {
            if (model == null || model.Id == null)
                return BadRequest();
            int addresult = 0;
            int subtract = 0;
            int changeRes = 0;
            try
            {
                var modelFull = await BuildByIdAdmin(model, String.Empty);
                if (modelFull == null) throw new Exception();

                if (modelFull != null && modelFull.BlockStateIds != null)
                {
                    foreach (var item in modelFull.BlockStateIds)
                    {
                        if (item == 1)
                        {
                            modelFull!.IsFullBlocked = false;
                            var sitem = modelFull?.BlockState?.First(x => x.IntId == 1);
                            sitem!.IsSelected = true;
                        }

                        if (item == 2)
                        {
                            modelFull!.IsFullBlocked = true;
                            var sitem = modelFull?.BlockState?.First(x => x.IntId == 1);
                            sitem!.IsSelected = true;
                        }

                    }
                    changeRes = await _userService.ChangeUserFullBlockById((Guid)model.Id, modelFull?.IsFullBlocked);
                }
                if (modelFull != null && modelFull.Id != null)
                {
                    if (modelFull.RoleIds != null)
                    {
                        foreach (var roleId in modelFull.RoleIds!)
                        {
                            var role = modelFull?.AllRoles?.FirstOrDefault(role => role.Id.Equals(roleId));

                            if (role != null)
                            {
                                role.IsSelected = true;
                                addresult += await _roleService.AddRoleByNameToUser((Guid)(modelFull?.Id!), role?.Name!);
                            }
                        }
                        foreach (var role in modelFull.AllRoles!)
                        {
                            if (modelFull!.RoleIds.All(r => r != role.Id))
                            {
                                role.IsSelected = false;
                                if (modelFull.RoleNames!.Any(x => x.Equals(role.Name)))
                                    subtract += await _roleService.RemoveRoleByNameFromUser((Guid)(modelFull?.Id!), role?.Name!);
                            }
                        }

                        modelFull.SystemInfo = $"<b>Roles:<br/>{addresult} was/were added<br/>{subtract} was/were deleted</b>";
                        if (changeRes > 0)
                        {
                            modelFull.SystemInfo += $"<br/><b>Full block status was changed</b>";
                        }
                        else
                        {
                            modelFull.SystemInfo += $"<br/><b>Full block status was not changed</b>";
                        }

                        return View(modelFull);
                    }

                    else
                    {
                        modelFull!.SystemInfo = $"<b>You have not chosen any role</b>";
                        return View(modelFull);
                    }
                }

                modelFull!.SystemInfo = "<b>Something went wrong (</b>";
               
                return View(modelFull);

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }


        private IQueryable<User> UserSort(IQueryable<User> users, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.EmailDesc:
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case SortState.NameAsc:
                    users = users.OrderBy(s => s.Name);
                    break;
                case SortState.NameDesc:
                    users = users.OrderByDescending(s => s.Name);
                    break;
                case SortState.SurnameAsc:
                    users = users.OrderBy(s => s.Surname);
                    break;
                case SortState.SurnameDesc:
                    users = users.OrderByDescending(s => s.Surname);
                    break;
                case SortState.BirtDateAsc:
                    users = users.OrderBy(s => s.BirthDate);
                    break;
                case SortState.BirthDateDesc:
                    users = users.OrderByDescending(s => s.BirthDate);
                    break;
                case SortState.LastLoginAsc:
                    users = users.OrderBy(s => s.LastLogin);
                    break;
                case SortState.LastLoginDesc:
                    users = users.OrderByDescending(s => s.LastLogin);
                    break;
                case SortState.IsFullBlockedAsc:
                    users = users.OrderBy(s => s.IsFullBlocked);
                    break;
                case SortState.IsFullBlockedDesc:
                    users = users.OrderByDescending(s => s.IsFullBlocked);
                    break;
                case SortState.IsFamilyDependentAsc:
                    users = users.OrderBy(s => s.IsDependent);
                    break;
                case SortState.IsFamilyDependentDesc:
                    users = users.OrderByDescending(s => s.IsDependent);
                    break;
                case SortState.IsOnlineAsc:
                    users = users.OrderBy(s => s.IsOnline);
                    break;
                case SortState.IsOnlineDesc:
                    users = users.OrderByDescending(s => s.IsOnline);
                    break;
                case SortState.GenderAsc:
                    users = users.OrderBy(s => s.Gender);
                    break;
                case SortState.GenderDesc:
                    users = users.OrderByDescending(s => s.Gender);
                    break;

                default:
                    users = users.OrderBy(s => s.Email);
                    break;
            }

            return users;
        }

        private IQueryable<User> UserFilter (IQueryable<User> users, string email, string name, string surname)
        {
            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(p => p.Email!.Contains(email));
            }
            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(p => p.Name!.Contains(name) || p.MidName!.Contains(name) || p.Surname!.Contains(name));
            }
            if (!string.IsNullOrEmpty(surname))
            {
                users = users.Where(p => p.Surname!.Contains(surname));
            }
            return users;
        }
        private IQueryable<DoctorData> DoctorDataFilter(IQueryable<DoctorData> dDatas, string email, string name, string surname, string speciality)
        {
            if (!string.IsNullOrEmpty(email))
            {
                dDatas = dDatas.Where(p => p.User!.Email!=null && p.User.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(name))
            {
                dDatas = dDatas.Where(p => p.User!.Name != null && p.User.Name.Contains(name)
                                        || p.User!.MidName != null && p.User.MidName.Contains(name)
                                        || p.User!.Surname != null && p.User.Surname.Contains(name));
            }
            if (!string.IsNullOrEmpty(surname))
            {
                dDatas = dDatas.Where(p => p.User!.Surname != null && p.User.Surname.Contains(surname));
            }
            if (!string.IsNullOrEmpty(speciality))
            {
                dDatas = dDatas.Where(p => p.Speciality!.Name != null && p.Speciality.Name.Contains(speciality));
            }
            return dDatas;
        }

        private IQueryable<DoctorData> DoctorDataSort(IQueryable<DoctorData> dDatas, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.EmailDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Email);
                    break;
                case SortState.NameAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.Name);
                    break;
                case SortState.NameDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Name);
                    break;
                case SortState.SurnameAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.Surname);
                    break;
                case SortState.SurnameDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Surname);
                    break;
                case SortState.BirtDateAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.BirthDate);
                    break;
                case SortState.BirthDateDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.BirthDate);
                    break;
                case SortState.LastLoginAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.LastLogin);
                    break;
                case SortState.LastLoginDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.LastLogin);
                    break;
                case SortState.IsFullBlockedAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.IsFullBlocked);
                    break;
                case SortState.IsFullBlockedDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.IsFullBlocked);
                    break;
                case SortState.IsBlockedAsc:
                    dDatas = dDatas.OrderBy(s => s.IsBlocked);
                    break;
                case SortState.IsBlockedDesc:
                    dDatas = dDatas.OrderByDescending(s => s.IsBlocked);
                    break;
                case SortState.IsMarkedAsc:
                    dDatas = dDatas.OrderBy(s => s.ForDeletion);
                    break;
                case SortState.IsMarkedDesc:
                    dDatas = dDatas.OrderByDescending(s => s.ForDeletion);
                    break;
                case SortState.IsFamilyDependentAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.IsDependent);
                    break;
                case SortState.IsFamilyDependentDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.IsDependent);
                    break;
                case SortState.IsOnlineAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.IsOnline);
                    break;
                case SortState.IsOnlineDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.IsOnline);
                    break;
                case SortState.GenderAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.Gender);
                    break;
                case SortState.GenderDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Gender);
                    break;
                case SortState.SpecialityAsc:
                    dDatas = dDatas.OrderBy(s => s!.Speciality!.Name);
                    break;
                case SortState.SpecialityDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.Speciality!.Name);
                    break;

                default:
                    dDatas = dDatas.OrderBy(s => s!.User!.Email);
                    break;
            }

            return dDatas;
        }
        private async Task<AdminUserEditModel?> BuildByIdAdmin(AdminUserEditModel model, string? id)
        {
            Guid userId = default;
            AdminUserEditModel? newModel = null;


            if (string.IsNullOrEmpty(id) && model.Id == null)
                return null;

            if (model.Id == null)
            {
                var res = Guid.TryParse(id, out Guid Id);
                if (!res)
                    return null;
                else userId = Id;
            }
            else
                userId = (Guid)model.Id;

            var usr = await _userService.GetUserByIdAsync(userId);
            var userRoles = await _roleService.GetRoleListByUserIdAsync(userId);
            var allroles = await _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToListAsync();
            if (usr == null && allroles == null) return null;

            newModel = _mapper.Map<AdminUserEditModel>(usr);
            if (newModel == null) return null;
            newModel.AllRoles = allroles;

            if (userRoles != null && userRoles.Any())
            {
                var roleList = userRoles.Select(r => r.Name).ToList();
                if (roleList != null && roleList.Any())
                    newModel!.RoleNames = roleList!;
            }


            if (model.Id != null)
            {
                newModel.BlockStateIds = model.BlockStateIds;
                newModel.RoleIds = model.RoleIds;
            }
            else
            {
                if (newModel.IsFullBlocked == true)
                {
                    var item = newModel?.BlockState?.First(x => x.IntId == 2);
                    item!.IsSelected = true;

                }
                else if (newModel.IsFullBlocked == false)
                {
                    var item = newModel?.BlockState?.First(x => x.IntId == 1);
                    item!.IsSelected = true;
                }
                else
                {
                    var item = newModel?.BlockState?.First(x => x.IntId == 0);
                    item!.IsSelected = true;
                }

                foreach (var item in newModel!.AllRoles)
                {
                    if (newModel.RoleNames!.Any(x => x.Equals(item.Name)))
                    {
                        item.IsSelected = true;
                    }
                }
            }

            return newModel;
        }

    }
}
