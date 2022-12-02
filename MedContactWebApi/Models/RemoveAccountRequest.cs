namespace MedContactWebApi.Models
{
    /// <summary>
    /// RemoveAccountRequest
    /// </summary>
    public class RemoveAccountRequest
    {
        ///
        public Guid? Id { get; set; } 
        ///
        public string? Pwd { get; set; } 
        ///
        public string? Keyword { get; set; } 
        ///
        public string? SystemInfo { get; set; }
    }
}
