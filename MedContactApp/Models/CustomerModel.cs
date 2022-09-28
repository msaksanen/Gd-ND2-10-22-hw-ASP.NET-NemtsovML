using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class CustomerModel
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(32)]
        public string? Username { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(32)]
        public string? Name { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(32)]
        public string? Surname { get; set; }
        public string? MidName { get; set; }

        [Required]
        public int? Age { get; set; }
        public string? Sex { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(256)]
        public string? Address { get; set; }

        [Required]
        public string? Role { get; set; }
        public DateTime RegistrationDate { get; set; }

    }
}
