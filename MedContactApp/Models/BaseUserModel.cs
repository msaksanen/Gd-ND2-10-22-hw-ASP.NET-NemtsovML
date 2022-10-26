using MedContactApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class BaseUserModel
    {
        public Guid? Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        public string? Username { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        [Remote(action: "CheckEmail", controller: "Customer", ErrorMessage = "Email is already in use")]
        public virtual  string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string? PasswordConfirmation { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }

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
        public virtual string? RoleName { get; set; }
        public DateTime? RegistrationDate { get; set; }

    }
}
