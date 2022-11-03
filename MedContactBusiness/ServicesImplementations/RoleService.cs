using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataAbstractions;
using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedContactBusiness.ServicesImplementations
{
    public class RoleService: IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetRoleNameByIdAsync(Guid id)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);

            if (role != null && role.Name!=null)
                return role.Name;

            return string.Empty;
        }

        public async Task<Guid?> GetRoleIdByNameAsync(string name)
        {
            var role = await _unitOfWork.RoleRepository.FindBy(role1 => role1.Name!=null && role1.Name.Equals(name))
                .FirstOrDefaultAsync();
            return role?.Id;
        }

        public async Task<RoleDto?> GetRoleByNameAsync(string name)
        {
           var role = await _unitOfWork.RoleRepository.FindBy(role1 => role1.Name != null && role1.Name.Equals(name))
                      .FirstOrDefaultAsync();
           var dto = _mapper.Map<RoleDto>(role);
          
           return dto;
        }
        public async Task<IEnumerable<RoleDto>?> GetRoleListByUserIdAsync (Guid id)
        {

            var list = await _unitOfWork.RoleRepository
                             .Get()
                             .AsNoTracking()
                             .Include(r => r.Users)
                             .AsNoTracking()
                             .ToListAsync();

            var lst = from role in list
                      from user in role.Users!
                      where user.Id.Equals(id)
                      select  _mapper.Map<RoleDto>(role);

            await _unitOfWork.Commit();

            return lst;
          
        }

        public IQueryable<Role> GetRoles()
        {
            return _unitOfWork.RoleRepository.Get();
        }

        public async Task<int> AddRoleByNameToUser (Guid userId, string roleName)
        {
            int result = 0;
            var userRoleList = await GetRoleListByUserIdAsync(userId);
            if (userRoleList != null && !userRoleList.Any(r => r.Name != null && r.Name.Equals(roleName)))
            {
                var role = await _unitOfWork.RoleRepository
                      .Get()
                      .FirstOrDefaultAsync(r => r.Name != null && r.Name.Equals(roleName));

                var user = await _unitOfWork.UserRepository.GetByIdTrackAsync(userId);

               _unitOfWork.RoleRepository
                           .Get()
                           .Include(r => r.Users)
                           .Load();

                if (role != null && user != null)
                {
                    role.Users?.Add(user);
                    result = await _unitOfWork.Commit();
                }
            }

            return result;

        }

        public async Task<int> RemoveRoleByNameFromUser(Guid userId, string roleName)
        {
            int result = 0;
            var userRoleList = await GetRoleListByUserIdAsync(userId);
            if (userRoleList != null && userRoleList.Any(r => r.Name != null && r.Name.Equals(roleName)))
            {
                var role = await _unitOfWork.RoleRepository
                      .Get()
                      .FirstOrDefaultAsync(r => r.Name != null && r.Name.Equals(roleName));

                var user = await _unitOfWork.UserRepository.GetByIdTrackAsync(userId);

                _unitOfWork.RoleRepository
                            .Get()
                            .Include(r => r.Users)
                            .Load();

                if (role != null && user != null)
                {
                    role.Users?.Remove(user);
                    result = await _unitOfWork.Commit();
                }
            }

            return result;

        }
        
    }
}
