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
        public List<FileData>? FileDatas { get; set; }

        //public string? FilePath { get; set; }
        public Guid? CustomerDataId { get; set; }
        public CustomerData? CustomerData { get; set; }
        public Guid? DoctorDataId { get; set; }
        public DoctorData? DoctorData { get; set; }
    }
}
