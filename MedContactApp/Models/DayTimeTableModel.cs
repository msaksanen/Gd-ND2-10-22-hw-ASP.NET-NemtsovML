using MedContactCore.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class DayTimeTableModel
    {
        public Guid? Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorMidName { get; set; }
        public string? DoctorSurname { get; set; }
        public string? DoctorSpeciality { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime? StartWorkTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime? FinishWorkTime { get; set; }

        [Required]
        [Range(1, 60)]
        public int? ConsultDuration { get; set; }
        public int? TotalTicketQty { get; set; }
        public int? FreeTicketQty { get; set; }
       
        [Required]
        public Guid? DoctorDataId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CustomerUserId { get; set; }
        public string? SystemInfo  { get; set; }
        public string Reflink { get; set; }  = string.Empty; 
    }
}
