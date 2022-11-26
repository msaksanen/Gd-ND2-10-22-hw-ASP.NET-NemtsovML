namespace MedContactWebApi.Models
{
    /// <summary>
    /// TokenResponse Model
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// string? AccessToken
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// string? []? Roles 
        /// </summary>
        public string?[]? Roles { get; set; }

        /// <summary>
        ///  Guid UserId
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        ///  DateTime TokenExpiration
        /// </summary>
        public DateTime TokenExpiration { get; set; }
        /// <summary>
        /// Guid RefreshToken
        /// </summary>
        public Guid RefreshToken { get; set; }
    }
}
