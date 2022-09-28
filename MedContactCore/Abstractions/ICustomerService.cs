using MedContactCore.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetCustomersByPageNumberAndPageSizeAsync
       (int pageNumber, int pageSize);

        List<CustomerDto> GetNewCustomersFromExternalSources();

        //Task<List<CustomerDto>> GetNewCustomersFromExternalSourcesAsync();

        Task<CustomerDto> GetCustomerByIdAsync(Guid id);

        Task<int> CreateCustomerAsync(CustomerDto dto);
        Task<int> PatchAsync(Guid id, List<PatchModel> patchList);
    }
}
