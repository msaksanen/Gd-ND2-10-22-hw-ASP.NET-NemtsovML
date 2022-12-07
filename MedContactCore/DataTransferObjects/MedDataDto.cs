using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class MedDataDto
    {
        public Guid Id { get; set; }
        public DateTime InputDate { get; set; }
        public string? Department { get; set; }
        public string? Type { get; set; }
        public string? ShortSummary { get; set; }
        public string? TextData { get; set; }

        //public string? FilePath { get; set; }
        public List<FileDataDto>? FileDatas { get; set; }
        public Guid? CustomerDataId { get; set; }
        public CustomerDataDto? CustomerData { get; set; }
        public Guid? DoctorDataId { get; set; }
        public DoctorDataDto? DoctorData{ get; set; }
    }
}
