using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IRoleAllUserService<DTO>
           where DTO : BaseUserDto     
    {
        Task<int> RegisterWithRoleAsync(DTO dto);
    }
}
