//using MedContactCore.DataTransferObjects;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;

//namespace MedContactApp.Models
//{
//    public class AddMedDataModel
//    {
//        public Guid? UserId { get; set; }
//        public Guid? MedDataId { get; set; }
//        public string? Email { get; set; }
//        public string? Name { get; set; }
//        public string? Surname { get; set; }
//        public string? MidName { get; set; }
//        public string? Department { get; set; }
//        public string? ShortSummary { get; set; }
//        public string? TextData { get; set; }
//        public Guid? CustomerDataId { get; set; }
//        public IFormFileCollection? Uploads { get; set; }
//        public int[]? MedDataTypeIds { get; set; }
//        public List<SelectItem> MedDataType{ get; set; } = new List<SelectItem>()
//        {
//            new SelectItem(){ IntId=0, Name="Note"},
//            new SelectItem(){ IntId=1, Name="Laboratory"},
//            new SelectItem(){ IntId=2, Name="Instrumental"},
//            new SelectItem(){ IntId=3, Name="Ultrasound"},
//            new SelectItem(){ IntId=4, Name="X-Ray"},
//            new SelectItem(){ IntId=5, Name="Endoscopy"},
//            new SelectItem(){ IntId=6, Name="Other"}
//        };
//        public string? SystemInfo { get; set; } =String.Empty;
//        public string? Reflink { get; set; }
//    }
//}


