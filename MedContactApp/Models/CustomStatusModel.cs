namespace MedContactApp.Models
{
    public class CustomStatusModel
    {
        public int?  Code { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Text { get; set; }
    }
}
