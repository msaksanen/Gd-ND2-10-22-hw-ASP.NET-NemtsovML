using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class MedDataInfo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime InputDate { get; set; }
        public string? Department { get; set; }
        public string? Type { get; set; }
        public string? ShortSummary { get; set; }
        public string? TextData { get; set; }
        public List<FileDataDto>? FileDatas { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorMidname { get; set; }
        public string? DoctorSurname { get; set; }
        public string? DoctorSpeciality { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerMidname { get; set; }
        public string? CustomerSurname { get; set; }
        public DateTime? CustomerBirthDate { get; set; }
        public Guid? CustomerDataId { get; set; }
        public Guid? DoctorDataId { get; set; }
    }
}
