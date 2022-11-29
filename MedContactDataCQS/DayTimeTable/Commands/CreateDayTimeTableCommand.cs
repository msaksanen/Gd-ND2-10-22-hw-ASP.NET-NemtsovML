using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DayTimeTable.Commands
{
    public class CreateDayTimeTableCommand : IRequest<Result>
    {
        public DayTimeTableDto? dayTimeTableDto { get; set; }
    }
}
