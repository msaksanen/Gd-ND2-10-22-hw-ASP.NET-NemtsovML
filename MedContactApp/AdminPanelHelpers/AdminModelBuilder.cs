using AutoMapper;
using MedContactApp.Helpers;
using MedContactApp.Models;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace MedContactApp.AdminPanelHelpers
{
    public class AdminModelBuilder
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


       
        public AdminModelBuilder(IUserService userService,
           IMapper mapper, IRoleService roleService, IConfiguration configuration,
           ModelUserBuilder modelBuilder, IFileDataService fileDataService, IDoctorDataService doctorDataService,
           IWebHostEnvironment appEnvironment, ISpecialityService specialityService)
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
        }
        internal async Task<AdminEditDoctorModel?> AdminDoctorModelBuildAsync(AdminEditDoctorModel smodel, Guid Uid,
          string? specr = "", string? specd = "")
        {
            var user = await _userService.GetUserByIdAsync(Uid);
            var doctorData = await _doctorDataService.GetDoctorDataListByUserId(Uid);
            var spec = await _specialityService.GetSpecialitiesListAsync();
            if (user != null && spec != null)
            {
                var model = _mapper.Map<AdminEditDoctorModel>(user);
                model.Specialities = spec;
                model.SpecialityIds = smodel.SpecialityIds;
                model.SpecialityBlockIds = smodel.SpecialityBlockIds;
                model.Reflink = smodel.Reflink;
                if (doctorData == null || doctorData.Count == 0)
                    return model;
                var resSpecR = Guid.TryParse(specr, out Guid specRId); //restore speciality
                var resSpecD = Guid.TryParse(specd, out Guid specDId); //delete speciality
                foreach (var item in doctorData)
                {
                    var sp = model?.Specialities?.FirstOrDefault(sp => sp.Id.Equals(item.SpecialityId));
                    if (sp != null)
                    {

                        if (resSpecR && sp.Id.Equals(specRId))
                        {
                            item.ForDeletion = false;
                            await _doctorDataService.UpdateDoctorDataAsync(item);
                        }

                        if (resSpecD && sp.Id.Equals(specDId))
                        {
                            await _doctorDataService.RemoveByIdAsync(item.Id);
                            continue;
                        }

                        if (item.ForDeletion == true)
                            sp.ForDeletion = true;
                        else
                            sp.IsSelected = true;

                        if (item.IsBlocked == true)
                            sp.IsSpecBlocked = true;
                    }
                }

                return model;
            }
            return null;
        }

        internal async Task<AdminUserEditModel?> AdminUserModelBuildAsync(AdminUserEditModel model, string? id)
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
            newModel.IsPwdReset = model.IsPwdReset;
            if (newModel == null) return null;
            newModel.AllRoles = allroles;

            if (userRoles != null && userRoles.Any())
            {
                var roleList = userRoles.Select(r => r.Name).ToList();
                if (roleList != null && roleList.Any())
                    newModel!.RoleNames = roleList!;
            }

            if(!string.IsNullOrEmpty(model.Reflink))
                newModel.Reflink = model.Reflink;   

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
