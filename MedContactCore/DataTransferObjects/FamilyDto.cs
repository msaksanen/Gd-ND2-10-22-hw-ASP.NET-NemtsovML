using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class FamilyDto
    {
        public Guid Id { get; set; }
        public Guid? MainUserId { get; set; }
        List<User>? Users { get; set; }
    }
}
