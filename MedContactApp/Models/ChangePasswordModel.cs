using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class ChangePasswordModel : BaseUserModel
    {
        //public Guid? Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //public string? Password { get; set; }

        //[Compare(nameof(Password))]
        //[DataType(DataType.Password)]
        //public string? PasswordConfirmation { get; set; }

        public string? SystemInfo { get; set; }

        public string? OldPwdInfo { get; set; }
    }
}
