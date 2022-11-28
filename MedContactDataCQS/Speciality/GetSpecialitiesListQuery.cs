using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Roles.Queries
{
    public class GetSpecialitiesListQuery : IRequest<List<SpecialityDto>?>
    {
    }
}
