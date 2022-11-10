using AutoMapper;
using MedContactApp.AdminPanelHelpers;
using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactApp.Helpers;
using MedContactApp.Models;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;
using System.Linq.Expressions;

namespace MedContactApp.Controllers
{
    public class AdminPanelDoctorController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly IFileDataService _fileDataService;
        private readonly IDoctorDataService _doctorDataService;
        private readonly ISpecialityService _specialityService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly AdminModelBuilder _adminModelBuilder;
        private readonly AdminSortFilter _adminSortFilter;
        private int _pageSize = 7;

        public AdminPanelDoctorController(IUserService userService,
            IMapper mapper, IRoleService roleService, IConfiguration configuration,
            ModelUserBuilder modelBuilder, IFileDataService fileDataService, IDoctorDataService doctorDataService,
            IWebHostEnvironment appEnvironment, ISpecialityService specialityService, AdminModelBuilder adminModelBuilder, 
            AdminSortFilter adminSortFilter)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
            _modelBuilder = modelBuilder;
            _fileDataService = fileDataService;
            _doctorDataService = doctorDataService;
            _appEnvironment = appEnvironment;
            _specialityService = specialityService;
            _adminModelBuilder = adminModelBuilder;
            _adminSortFilter = adminSortFilter; 
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

                IQueryable<User> doctors = _userService.GetUsers()
                                           .Include(u => u.Roles)
                                           .Include(u => u.DoctorDatas);

                var newdoctors = from d in doctors
                                 from r in d.Roles!
                                 where d.DoctorDatas == null || d.DoctorDatas.Count == 0
                                 where r.Name!.Equals("Doctor")
                                 select d;

                newdoctors = _adminSortFilter.UserSort(newdoctors, sortOrder);
                var nList = await newdoctors
                                  .Select(d => _mapper.Map<DoctorFullDataDto>(d))
                                  .ToListAsync();

                //IQueryable<DoctorData> datas = _doctorDataService
                //                                 .GetDoctorData()
                //                                 .Include(d => d.Speciality)
                //                                 .Include(d => d.DayTimeTables);

                //var doctorfulldata = from doct in doctors
                //                     from d in doct.DoctorDatas!
                //                     join s in datas on d.UserId equals s.UserId
                //                     select new 
                //                     { 
                //                        Id =doct.Id, Email=doct.Email,
                //                        Username=doct.Name, Surname=doct.Surname, MidName=doct.MidName,
                //                        BirthDate = doct.BirthDate, Gender= doct.Gender,
                //                        IsFullBlocked =doct.IsFullBlocked,
                //                        DoctorDatas=d, 
                //                     };



                var roles = _roleService.GetRoles().Select(role => _mapper.Map<RoleDto>(role)).ToList();

                if (roleId != default)
                {
                    dDatas = (from d in dDatas
                              from r in d.User!.Roles!
                              where r.Id.Equals(roleId)
                              select d);
                }

                dDatas = _adminSortFilter.DoctorDataFilter(dDatas, email, name, surname, speciality);
                dDatas = _adminSortFilter.DoctorDataSort(dDatas, sortOrder);

                var dataList = await dDatas.ToListAsync();
                var list = dataList.Select(dd => _mapper.Map<DoctorFullDataDto>((dd, dd.User, dd.Speciality))).ToList();
                var fulList = nList.Concat(list);

                var count = fulList.Count();
                var items = fulList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                string pageRoute = @"/adminpaneldoctor/doctordataindex?page=";
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

                applicants = _adminSortFilter.UserFilter(applicants, email, name, surname);
                applicants = _adminSortFilter.UserSort(applicants, sortOrder);

                var count = await applicants.CountAsync();
                var items = await applicants.Skip((page - 1) * pageSize).Take(pageSize)
                           .Select(user => _mapper.Map<UserDto>(user)).ToListAsync();

                string pageRoute = @"/adminpaneldoctor/applicationindex?page=";
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
        public async Task<IActionResult> ApplicantDetails(string? id, string? filedata = "")
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var res = Guid.TryParse(id, out Guid Id);
            if (!res)
                return BadRequest();
            var usr = await _userService.GetUserByIdAsync(Id);
            if (usr == null)
                return NotFound();

            var model = _mapper.Map<ApplicantModel>(usr);

            if (!string.IsNullOrEmpty(filedata))
            {
                var resfile = Guid.TryParse(filedata, out Guid fdId);
                if (!resfile)
                    model.SystemInfo = $"<b>File data Id({filedata}) for deletion is incorrect.</b>";
                var removeResult = await _fileDataService.RemoveFileDataWithFileById(fdId, _appEnvironment.WebRootPath);

                switch (removeResult)
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
            if (fileList != null && fileList.Any())
                fileList = fileList.Where(x => x.Category != null && x.Category.Equals("Applicant")).ToList();
            model.fileDatas = fileList;

            var doctInfoList = await _doctorDataService.GetDoctorInfoByUserId(Id);
            if (doctInfoList != null && doctInfoList.Any())
                model.doctorInfos = doctInfoList;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DoctorDetails(string? userid, string? specr, string? specd)
        {
            if (string.IsNullOrEmpty(userid))
                return NotFound();
            var res = Guid.TryParse(userid, out Guid userId);

            if (!res)
                return NotFound("User Id is incorrect");
            try
            {
                var model = await _adminModelBuilder.AdminDoctorModelBuildAsync(new AdminEditDoctorModel(), userId, specr, specd);
                if (model != null)
                {
                    return View(model);
                }
                return BadRequest();
            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DoctorDetails(AdminEditDoctorModel model)
        {
            if (model == null && model?.UserId == null)
                return BadRequest();

            int addresult = 0;
            int updresult = 0;
            int subtract = 0;
         try
         { 
            var modelFull = await _adminModelBuilder.AdminDoctorModelBuildAsync(model, (Guid)model.UserId!);
            if (modelFull == null)
                return BadRequest();
            var doctorData = await _doctorDataService.GetDoctorDataListByUserId((Guid)model.UserId);
            var roleId = await _roleService.GetRoleIdByNameAsync("Doctor");
           
            if (modelFull.SpecialityIds != null)
            {
                foreach (var sp in modelFull.SpecialityIds)
                {
                    if (doctorData.All(ddt => ddt.SpecialityId != sp))
                    {
                        var specModel = modelFull?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(sp));
                        if (specModel != null) specModel.IsSelected = true;

                        DoctorDataDto doctorDataDto = new()
                        {
                            Id = Guid.NewGuid(),
                            IsBlocked = false,
                            UserId = modelFull?.UserId,
                            SpecialityId = sp,
                            RoleId = roleId,
                            SpecNameReserved = specModel?.Name
                        };
                        addresult += await _doctorDataService.CreateDoctorDataAsync(doctorDataDto);
                    }
                }
            }
            foreach (var dd in doctorData)
            {
                if (modelFull?.SpecialityIds == null || modelFull!.SpecialityIds.All(spec => spec != dd.SpecialityId))
                {
                    var specModel = modelFull?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(dd.SpecialityId) && dd.ForDeletion != true);
                    if (specModel != null)
                    {
                        specModel.IsSelected = false;
                        specModel.ForDeletion = true;
                        subtract += await _doctorDataService.MarkForDeleteDoctorDataAsync(dd);
                    }
                }
            }

            if (modelFull.SpecialityBlockIds != null)
            {
                foreach (var sp in modelFull.SpecialityBlockIds)
                {
                    var blockdata = doctorData.FirstOrDefault(d => d.SpecialityId.Equals(sp) && d.IsBlocked == false);
                    if (blockdata != null)
                    {
                        blockdata.IsBlocked = true;
                        var specModel = modelFull?.Specialities?.FirstOrDefault(s => s.Id.Equals(sp));
                        if (specModel != null) specModel.IsSpecBlocked = true;
                        updresult += await _doctorDataService.UpdateDoctorDataAsync(blockdata);
                    }
                }
            }

            foreach (var dd in doctorData)
            {
                if (modelFull?.SpecialityBlockIds == null || modelFull!.SpecialityBlockIds.All(spec => spec != dd.SpecialityId))
                {
                    if (dd.IsBlocked == true)
                    {
                        dd.IsBlocked = false;
                        var specModel = modelFull?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(dd.SpecialityId));
                        if (specModel != null) specModel.IsSpecBlocked = false;

                        updresult += await _doctorDataService.UpdateDoctorDataAsync(dd);
                    }
                }
            }

            modelFull.SystemInfo = $"<b>Specialities:<br/>{addresult} was/were added" +
                                   $"<br/>{subtract} was/were deleted<br/>Blockstate of {updresult} was/were updated</b>";

            return View(modelFull);
        }
        catch (Exception e)
        {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
        }
    }

        [HttpGet]
        public async Task<IActionResult> SpecialityManager(string? delid, string? newspeciality)
        {
            AdminSpecModel model = new();
         
            if (!string.IsNullOrEmpty(newspeciality))
            {
                SpecialityDto newSpeciality = new() { Id = Guid.NewGuid(), Name = newspeciality };
                var addres = await _specialityService.AddSpecialityToDb(newSpeciality);
                if (addres > 0)
                    model.SystemInfo = $"<b>Speciality {newspeciality} was added to database.</b>";
                if (addres == 0)
                    model.SystemInfo = $"<b> No speciality was added to database.</b>";
            }

            if (!string.IsNullOrEmpty(delid))
            {
                var res = Guid.TryParse(delid, out var id);
                if (res)
                {
                    var delres = await _specialityService.RemoveSpecialityById(id);
                    if (delres.IntResult > 0)
                        model.SystemInfo += $"<br/><b>Speciality {delres.Name} was removed from database.</b>";
                }
            }

            var specList = await _specialityService.GetSpecialitiesListAsync();

            if (specList != null)
            {

                model.Specialities = specList;
            }

            return View(model);
        }     
    }
}