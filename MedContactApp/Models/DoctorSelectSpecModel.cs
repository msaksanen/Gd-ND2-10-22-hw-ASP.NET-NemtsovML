using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class DoctorSelectSpecModel
    {
        public UserDto? User { get; set; }
        public List<DoctorInfo>? DoctorInfos { get; set; }
        public string? SystemInfo { get; set; }
        public int Flag { get; set; } = 1;

    }
}
