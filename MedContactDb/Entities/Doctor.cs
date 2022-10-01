using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedContactDb.Entities
{
    public class Doctor : BaseUser
    {
        public string? Speciality { get; set; }
        public List<DayTimeTable>? DayTimeTables { get; set; }
    }
}
