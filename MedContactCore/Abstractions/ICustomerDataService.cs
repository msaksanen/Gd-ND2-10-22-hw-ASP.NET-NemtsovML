using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface ICustomerDataService
    {
        //Task<Result> CreateByUserIdAsync(Guid userId);
        Task<CustomerDataDto?> CreateByUserIdAsync(Guid userId);
        //Task<CustomerDataDto?> GetOrCreateByUserIdAsync(Guid userId);
        Task<CustomerDataDto?> GetByIdAsync(Guid customerDataId);
    }
}
