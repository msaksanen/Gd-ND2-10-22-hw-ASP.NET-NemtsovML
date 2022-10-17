using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class ExtraDataDto
    {
        public Guid Id { get; set; }
        public Guid? AcsDataId { get; set; }
        public string? PropName { get; set; }
        public string? PropStringValue { get; set; }
        public int? PropIntValue { get; set; }
        public bool? IsPropBlocked { get; set; }

    }
}
