using MedContactCore.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// DayTimeTableModel
    /// </summary>
    public class DayTimeTableModel
    {
        ///
        public Guid? Id { get; set; }
        ///
        public DateTime CreationDate { get; set; }
        ///
        public string? DoctorName { get; set; }
        ///
        public string? DoctorMidName { get; set; }
        ///
        public string? DoctorSurname { get; set; }
        ///
        public string? DoctorSpeciality { get; set; }
        ///
        public DateTime? Date { get; set; }
        ///
        public DateTime? StartWorkTime { get; set; }
        ///
        public DateTime? FinishWorkTime { get; set; }
        ///
        public int? ConsultDuration { get; set; }
        ///
        public int? TotalTicketQty { get; set; }
        ///
        public int? FreeTicketQty { get; set; }
        ///
        public Guid? DoctorDataId { get; set; }
        ///
        public Guid? UserId { get; set; }
        ///
        public string? SystemInfo  { get; set; }
    }
}
