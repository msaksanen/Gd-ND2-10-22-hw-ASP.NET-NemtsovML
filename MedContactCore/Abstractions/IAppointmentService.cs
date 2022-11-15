using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IAppointmentService
    {
       Task<List<AppointmentDto>?> GetAppointmentsByDTTableIdAsync(Guid dttId);
       Task<int> UpdateRange(IEnumerable<AppointmentDto> apms);
       Task<int> Add(AppointmentDto apm);
       Task<IEnumerable<AppointmentDto>?> GetAppointmentsByUserId(Guid usrId);



    }
}
