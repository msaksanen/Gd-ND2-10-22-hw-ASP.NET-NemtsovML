namespace MedContactApp.Models
{
    public class UserDataModel
    {
        public Guid Id { get; set; }
        public string? ActiveFullName { get; set; }
        public string? ActiveEmail { get; set; }
        public string? MainFullName { get; set; }
        public string? MainEmail { get; set; }
        public List<string>? RoleNames { get; set; }
    }
}
