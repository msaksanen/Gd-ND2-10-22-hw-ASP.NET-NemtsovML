using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedContactCore.Abstractions;
using MedContactDataAbstractions;
using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedContactBusiness.ServicesImplementations
{
    public class RoleService: IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {

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

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
           return await _unitOfWork.RoleRepository.FindBy(role1 => role1.Name != null && role1.Name.Equals(name))
              .FirstOrDefaultAsync();
        }
    }
}
