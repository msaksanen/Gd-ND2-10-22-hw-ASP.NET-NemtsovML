using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class DayTimeTableModel
    {
        public Guid? Id { get; set; }
        
        [Required]
        public DateTime? Date { get; set; }
        [Required]
        public DateTime? StartWorkTime { get; set; }
        [Required]
        public DateTime? FinishWorkTime { get; set; }
        [Required]
        [Range(1, 60)]
        public int? ConsultDuration { get; set; }
        public int? TotalTicketQty { get; set; }
        public int? FreeTicketQty { get; set; }
       
        [Required]
        public Guid? DoctorId { get; set; }
    }
}
