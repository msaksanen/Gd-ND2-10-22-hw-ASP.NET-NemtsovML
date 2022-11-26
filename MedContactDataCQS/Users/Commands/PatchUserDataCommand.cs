using MedContactCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Commands
{
    public class PatchUserDataCommand : IRequest<int?>
    {
        public Guid? UserId { get; set; }
        public  List<PatchModel>? PatchList { get; set; }
    }
}
