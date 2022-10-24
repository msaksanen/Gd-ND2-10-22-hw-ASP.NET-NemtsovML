using MedContactCore.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class EditDoctorDataModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? UserId { get; set; }
        public bool? IsBlocked { get; set; }
        public Guid[]? SpecialityIds { get; set; }
        public List <SpecialityDto>? Specialities { get; set; }
        public string? SystemInfo { get; set; }
    }
}
