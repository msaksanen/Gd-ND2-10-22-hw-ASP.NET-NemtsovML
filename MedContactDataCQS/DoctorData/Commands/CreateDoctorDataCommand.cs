using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DoctorData.Commands
{
    public class CreateDoctorDataCommand : IRequest<int>
    {
        public DoctorDataDto? DoctorDataDto { get; set; }
    }
}
