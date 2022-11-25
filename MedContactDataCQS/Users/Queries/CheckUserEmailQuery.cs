using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Queries
{
    public  class CheckUserEmailQuery : IRequest<bool?>
    {
        public string? Email { get; set; }
    }
}
