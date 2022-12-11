using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IUserService
    {
        Task<List<UserDto>> GetUsersByPageNumberAndPageSizeAsync
       (int pageNumber, int pageSize);
        Task<int> GetUserEntitiesCountAsync();
        List<UserDto> GetNewUsersFromExternalSources();
        Task<bool> IsUserExists(Guid userId);
        Task<bool> CheckUserPassword(string email, string password);
        Task<bool> CheckUserPassword(Guid userId, string password);
        Task<int> ChangeUserPasswordAsync(Guid id, string password);
        Task<Result> GetIdByEmailUserPassword(string email, string password);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> GetUserByIdEmailAsync(string email, Guid id);
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<int> CreateUserAsync(UserDto dto);
        Task<int> CreateUserWithRoleAsync(UserDto dto,string roleName);
        Task<bool> CheckUserEmailAsync(string email);
        Task<int> PatchAsync(Guid id, List<PatchModel> patchList);
        IQueryable<User> GetUsers();
        Task<int> ChangeUserStatusById(Guid id, int state);
        Task<int> ChangeUserFullBlockById(Guid id, bool? IsFullBlocked);
        Task<int> CheckUserByUsernamePassword(Guid id, string pwd, string username);
        Task<int> RemoveUserById(Guid id);
        IQueryable<User> GetDoctorsWithFullData();
        IQueryable<User> GetNewDoctors();
        IQueryable<User> GetApplicants();
    }
}
