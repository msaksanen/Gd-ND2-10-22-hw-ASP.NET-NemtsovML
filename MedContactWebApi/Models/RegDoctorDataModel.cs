using MedContactCore.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// RegDoctorDataModel
    /// </summary>
    public class RegDoctorDataModel
    { 
        ///
        public string? SystemInfo { get; set; }
        ///
        public IFormFileCollection? Uploads { get; set; }
        ///
        public string? Password { get; set; }
    }
}
