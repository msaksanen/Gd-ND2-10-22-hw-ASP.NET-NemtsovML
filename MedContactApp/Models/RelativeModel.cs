using MedContactApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class RelativeModel
    {
        public Guid? Id { get; set; }
        public string? PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string? Email { get; set; }

        [MaxLength(32)]
        public string? Name { get; set; }

        [MaxLength(32)]
        public string? Surname { get; set; }
        public string? MidName { get; set; }

        //[Required]
        //[Range(1, 110)]
        //public int? Age { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Remote(action: "CheckBirthDate", controller: "Customer", ErrorMessage = "Input correct date of birth")]
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string? Address { get; set; }
        public Guid? FamilyId { get; set; }
        public Guid? CustomerDataId { get; set; }
        public bool? IsDependent { get; set; }
        public bool? IsFullBlocked { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; } = "Customer";
        public DateTime? RegistrationDate { get; set; }
      
      

    }
}
