using MedContactCore.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface ISpecialityService
    {
        Task<List<SpecialityDto>?> GetSpecialitiesListAsync();
        Task<int> DeleteSpecialityByName(string? name);
    }
}
