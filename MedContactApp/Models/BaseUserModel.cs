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
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string? PasswordConfirmation { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        public string? Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        public string? Surname { get; set; }
        public string? MidName { get; set; }

        [Required]
        public int? Age { get; set; }
        public string? Sex { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string? Address { get; set; }
        public string? Role { get; set; }
        public DateTime? RegistrationDate { get; set; }

    }
}
