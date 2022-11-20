namespace MedContactApp.Models
{
    public class CustomExceptionModel
    {
        public string?  ActionName { get; set; }
        public string? ExceptionStack { get; set; }
        public string? ExceptionMessage { get; set; }
    }
}
