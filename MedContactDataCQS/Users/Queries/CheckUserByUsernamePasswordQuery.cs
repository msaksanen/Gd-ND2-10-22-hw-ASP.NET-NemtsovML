using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Queries
{
    public class CheckUserByUsernamePasswordQuery : IRequest<int>
    {
        public Guid? Id { get; set; } 
        public string? PwdHash { get; set; } 
        public string? Username { get; set; }
    }
}
