using MedContactCore.DataTransferObjects;

namespace MedContactApp.Models
{
    public class ApplicantModel:BaseUserModel
    {
        public List<FileDataDto>? fileDatas { get; set; }
        public List<DoctorInfo>? doctorInfos { get; set; }
        public string? SystemInfo { get; set; }
        public string? Reflink { get; set; }
    }
}
