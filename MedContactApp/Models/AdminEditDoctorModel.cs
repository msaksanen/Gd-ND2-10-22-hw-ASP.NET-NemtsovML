using MedContactCore.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class AdminEditDoctorModel
    {
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? MidName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public bool? IsFullBlocked { get; set; }
        public Guid[]? SpecialityIds { get; set; }
        public Guid[]? SpecialityBlockIds { get; set; }
        public List <SpecialityDto>? Specialities { get; set; }
        public string? SystemInfo { get; set; }
    }
}
