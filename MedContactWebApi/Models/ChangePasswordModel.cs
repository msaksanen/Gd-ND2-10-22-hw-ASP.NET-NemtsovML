using System.ComponentModel.DataAnnotations;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// ChangePasswordModel
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        public Guid? Id { get; set; }
        ///
        public string? Password { get; set; }
        ///
        public string? PasswordConfirmation { get; set; }
        ///
        public string? OldPassword { get; set; }
        ///
        public string? SystemInfo   { get; set; }

    }
}
