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

        // Task<List<UserDto>> GetUsersByPageNumberAndPageSizeAsync
        //(int pageNumber, int pageSize);
        // Task<int> GetUserEntitiesCountAsync();
        // List<UserDto> GetNewUsersFromExternalSources();
        // Task<bool> IsUserExists(Guid userId);
        // Task<bool> CheckUserPassword(string email, string password);
        // Task<bool> CheckUserPassword(Guid userId, string password);
        // Task<UserDto> GetUserByEmailAsync(string email);
        // Task<UserDto> GetUserByIdAsync(Guid id);
        // Task<int> CreateUserAsync(UserDto dto);
        // Task<int> CreateUserWithRoleAsync(UserDto dto,string roleName);
        // Task<bool> CheckUserEmailAsync(string email);
        // Task<int> PatchAsync(Guid id, List<PatchModel> patchList);
    }
}
