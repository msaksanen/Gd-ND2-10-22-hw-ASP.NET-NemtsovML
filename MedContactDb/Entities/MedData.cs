using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class MedData : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime InputDate { get; set; }
        public string? Department { get; set; }
        public string? Type { get; set; }
        public string? ShortSummary { get; set; }
        public string? TextData { get; set; }
        public string? FilePath { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
