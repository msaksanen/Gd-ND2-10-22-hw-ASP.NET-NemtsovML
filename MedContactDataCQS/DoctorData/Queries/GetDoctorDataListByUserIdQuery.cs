using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DoctorData.Queries
{
    public class GetDoctorDataListByUserIdQuery : IRequest<List<DoctorDataDto>?>
    {
        public Guid? UserId  { get; set; }   
    }
}
