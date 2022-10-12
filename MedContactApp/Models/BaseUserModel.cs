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

        [Required]
        [Range(1, 110)]
        public int? Age { get; set; }
        public string? Sex { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string? Address { get; set; }
        public bool? IsBlocked { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateTime? RegistrationDate { get; set; }

    }
}
