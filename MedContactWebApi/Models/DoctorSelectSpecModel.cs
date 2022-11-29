using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// DoctorSelectSpecModel
    /// </summary>
    public class DoctorSelectSpecModel
    {
        ///
        public UserDto? User { get; set; }
        ///
        public List<DoctorInfo>? DoctorInfos { get; set; }
        ///
        public string? SystemInfo { get; set; }

    }
}
