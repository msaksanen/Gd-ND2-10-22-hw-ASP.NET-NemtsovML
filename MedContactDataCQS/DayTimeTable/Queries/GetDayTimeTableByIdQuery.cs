using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedContactCore.DataTransferObjects;

namespace MedContactDataCQS.DayTimeTable.Queries
{
    public class GetDayTimeTableByIdQuery : IRequest<DayTimeTableDto?>
    {
        public Guid? DttId { get; set; }
   }
}
