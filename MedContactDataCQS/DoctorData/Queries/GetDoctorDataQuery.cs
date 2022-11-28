using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DoctorData.Queries
{
    public class GetDoctorDataQuery : IRequest<IQueryable<MedContactDb.Entities.DoctorData>?>
    {
    }
}
