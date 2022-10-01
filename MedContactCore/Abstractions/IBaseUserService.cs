using MedContactCore.DataTransferObjects;
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

        //Task<List<CustomerDto>> GetNewCustomersFromExternalSourcesAsync();

        Task<DTO> GetBaseUserByIdAsync(Guid id);

        Task<int> CreateBaseUserAsync(DTO dto);
        Task<int> PatchAsync(Guid id, List<PatchModel> patchList);
    }
}
