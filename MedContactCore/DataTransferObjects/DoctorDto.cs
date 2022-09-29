using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class DoctorDto : BaseUserDto
    {
        public string? Speciality { get; set; }
        public List<DayTimeTableDto>? DayTimeTables { get; set; }
    }
}
