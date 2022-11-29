using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DayTimeTable.Queries
{
    public class GetDayTimeTableListByDoctorDataIdQuery : IRequest<IEnumerable<DayTimeTableDto>?>
    {
        public Guid? DoctorDataId { get; set; } 
    }
}
