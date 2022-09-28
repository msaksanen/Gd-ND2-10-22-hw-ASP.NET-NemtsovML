using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class DoctorDto : BaseUserDto
    {
        public List<DayTimeTableDto>? DayTimeTables { get; set; }
    }
}
