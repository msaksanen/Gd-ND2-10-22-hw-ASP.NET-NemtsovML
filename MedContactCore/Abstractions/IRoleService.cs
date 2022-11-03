﻿using MedContactCore.DataTransferObjects;
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
        Task<IEnumerable<RoleDto>?> GetRoleListByUserIdAsync (Guid id);
        Task<int> AddRoleByNameToUser (Guid userId, string roleName);
        Task<int> RemoveRoleByNameFromUser(Guid userId, string roleName);
        IQueryable<Role> GetRoles();
    }
}
