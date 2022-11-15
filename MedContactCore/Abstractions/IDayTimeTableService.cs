using MedContactCore.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IDayTimeTableService
    {
      Task<int> CreateDayTimeTableAsync(DayTimeTableDto dto);
      Task<List<DayTimeTableDto>> GetDayTimeTableByPageNumberAndPageSizeAsync(int pageNumber, int pageSize);
      Task<int> GetDayTimeTableEntitiesCountAsync();
      Task<IEnumerable<DayTimeTableDto>?> GetDayTimeTableByDoctorDataId(Guid dataId);
      Task<DayTimeTableDto?> GetDayTimeTableByIdAsync(Guid dttId);
    }
}
