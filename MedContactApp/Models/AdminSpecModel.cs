using MedContactCore.DataTransferObjects;

namespace MedContactApp.Models
{
    public class AdminSpecModel
    {
        public string? NewSpeciality { get; set; }  
        public  List<SpecialityDto>? Specialities { get; set; }  
        public string? SystemInfo { get; set; }
    }
}
