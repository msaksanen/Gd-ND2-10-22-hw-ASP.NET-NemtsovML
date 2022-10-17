using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IRoleService
    {
        Task<string> GetRoleNameByIdAsync(Guid id);
        Task<Guid?> GetRoleIdByNameAsync(string name);
        Task<RoleDto?> GetRoleByNameAsync(string name);
        Task<List<RoleDto>?> GetRoleListByUserIdAsync (Guid id);
    }
}
