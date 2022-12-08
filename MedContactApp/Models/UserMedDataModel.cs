using MedContactCore.DataTransferObjects;

namespace MedContactApp.Models
{
    public class UserMedDataModel
    {
        public Guid? UserId { get; set; }
        public Guid? FamilyId { get; set; }
        public Guid? MedDataId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? MidName { get; set; }
        public DateTime InputDate { get; set; }
        public string? Type { get; set; }
        public string? Department { get; set; }
        public string? ShortSummary { get; set; }
        public string? TextData { get; set; }
        public Guid? CustomerDataId { get; set; }
        public Guid? DoctorDataId { get; set; }
        public string? SystemInfo { get; set; } = String.Empty;
        public string? Reflink { get; set; }
        public List<FileDataDto>? fileDatas { get; set; }
        public object? ErrorObject   { get; set; }
        public int Flag { get; set; } = 0;
        public IFormFileCollection? Uploads { get; set; }
        public int[]? MedDataTypeIds { get; set; }
        public List<SelectItem> MedDataType { get; set; } = new List<SelectItem>()
        {
            new SelectItem(){ IntId=0, Name="Note"},
            new SelectItem(){ IntId=1, Name="Laboratory"},
            new SelectItem(){ IntId=2, Name="Instrumental"},
            new SelectItem(){ IntId=3, Name="Ultrasound"},
            new SelectItem(){ IntId=4, Name="X-Ray"},
            new SelectItem(){ IntId=5, Name="Endoscopy"},
            new SelectItem(){ IntId=6, Name="Clinical"},
            new SelectItem(){ IntId=7, Name="Message"},
            new SelectItem(){ IntId=8, Name="Other"}
        };
    }
}
