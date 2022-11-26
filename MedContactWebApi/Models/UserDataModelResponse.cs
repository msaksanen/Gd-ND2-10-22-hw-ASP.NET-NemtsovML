namespace MedContactWebApi.Models
{
    /// <summary>
    /// UserDataModel for UserPreview
    /// </summary>
    public class UserDataModelResponse
    {

        /// <summary>
        /// Active User FullName
        /// </summary>
        public string? ActiveFullName { get; set; }
        /// <summary>
        /// Active User Email
        /// </summary>
        public string? ActiveEmail { get; set; }
        /// <summary>
        /// Main User FullName
        /// </summary>
        public string? MainFullName { get; set; }
        /// <summary>
        /// Main User Email
        /// </summary>
        public string? MainEmail { get; set; }
        /// <summary>
        /// Main User RoleNames
        /// </summary>
        public List<string>? RoleNames { get; set; }
    }
}
