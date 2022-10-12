using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IBaseUserService<DTO>  where DTO : BaseUserDto
    {
        Task<List<DTO>> GetBaseUsersByPageNumberAndPageSizeAsync
       (int pageNumber, int pageSize);
        Task<int> GetBaseUserEntitiesCountAsync();
        List<DTO> GetNewBaseUsersFromExternalSources();

        Task<bool> IsBaseUserExists(Guid userId);
        Task<bool> CheckBaseUserPassword(string email, string password);
        Task<bool> CheckBaseUserPassword(Guid userId, string password);
       //Task<int> RegisterBaseUser(DTO dto);
        Task<DTO> GetBaseUserByEmailAsync(string email);

        //Task<List<CustomerDto>> GetNewCustomersFromExternalSourcesAsync();

        Task<DTO> GetBaseUserByIdAsync(Guid id);
        Task<int> CreateBaseUserAsync(DTO dto);
        Task<bool> CheckBaseUserEmailAsync(string email);
        Task<int> PatchAsync(Guid id, List<PatchModel> patchList);
    }
}
