using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IFamilyService
    {
        Task<List<UserDto>?> GetRelativesAsync(Guid mainId);
        Task<int> CreateNewFamilyForMainUser(UserDto mainUserDto, FamilyDto familyDto);

    }
}
