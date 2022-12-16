using MedContactCore.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class CustomerDataDto
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? UserId { get; set; }
        public UserDto? User { get; set; }
        public bool? IsBlocked { get; set; }
        public List<AppointmentDto>? Appointments { get; set; }
        public List<MedDataDto>? MedData { get; set; }
    }
}
