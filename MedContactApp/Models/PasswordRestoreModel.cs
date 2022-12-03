using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class PasswordRestoreModel
    {
        public Guid? Id { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public virtual string? Email { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        public string? Codeword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string? PasswordConfirmation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Remote(action: "CheckBirthDate", controller: "Customer", ErrorMessage = "Input correct date of birth")]
        public DateTime? BirthDate { get; set; }

        public string? SystemInfo { get; set; }=String.Empty;   
    }
}
