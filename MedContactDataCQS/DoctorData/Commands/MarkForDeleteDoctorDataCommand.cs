using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DoctorData.Commands
{
    public class MarkForDeleteDoctorDataCommand : IRequest<int>
    {
        public DoctorDataDto? DoctorDataDto { get; set; }
    }
}
