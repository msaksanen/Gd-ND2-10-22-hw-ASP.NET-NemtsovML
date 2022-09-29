using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class DoctorModel:BaseUserModel
    {
        [Required]
        public string? Speciality { get; set; }

    }
}
