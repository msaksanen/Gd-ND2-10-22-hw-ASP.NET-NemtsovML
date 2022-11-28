using MedContactCore.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// EditDoctorDataResponse
    /// </summary>
    public class EditDoctorDataResponse
    {
        ///
        public string? Password { get; set; }
        ///
        public Guid[]? SpecialityIds { get; set; }

    }
}
