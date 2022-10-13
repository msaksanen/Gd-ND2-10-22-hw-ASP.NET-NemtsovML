using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class CustomerModel:BaseUserModel
    {
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        [Remote(action: "CheckEmail", controller: "Customer", ErrorMessage = "Email is already in use")]
        public override string? Email { get; set; }
        public override string? RoleName { get; set; } = "Customer";
        public override string? EntityName { get; set; } = "Customer";

    }
}
