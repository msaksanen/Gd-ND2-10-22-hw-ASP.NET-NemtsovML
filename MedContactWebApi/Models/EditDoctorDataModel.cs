using MedContactCore.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// EditDoctorDataModel
    /// </summary>
    public class EditDoctorDataModel
    {
        ///
        public string? Password { get; set; }
        ///
        public Guid? RoleId { get; set; }
        ///
        public Guid? UserId { get; set; }
        ///
        public bool? IsBlocked { get; set; }
        ///
        public Guid[]? SpecialityIds { get; set; }
        ///
        public List <SpecialityDto>? Specialities { get; set; }
        ///
        public string? SystemInfo { get; set; }
    }
}
