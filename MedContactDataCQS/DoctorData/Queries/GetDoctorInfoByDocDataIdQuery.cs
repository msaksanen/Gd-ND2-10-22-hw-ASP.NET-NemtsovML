using MedContactCore.DataTransferObjects;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DoctorData.Queries
{
    public class GetDoctorInfoByDocDataIdQuery : IRequest<DoctorInfo?>
    {
        public Guid? DoctorDataId { get; set; }   
    }
}
