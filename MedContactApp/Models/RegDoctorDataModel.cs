using MedContactCore.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class RegDoctorDataModel : BaseUserModel
    {
        //[Required]
        //[DataType(DataType.Password)]
        //public string? Password { get; set; }
        //public Guid? RoleId { get; set; }
        //public Guid? UserId { get; set; }
        public string? SystemInfo { get; set; }
        public IFormFileCollection? Uploads { get; set; }
    }
}
