namespace MedContactWebApi.Helpers
{
    /// <summary>
    /// MD5 Encoding
    /// </summary>
    public class MD5
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// MD5 Ctor
        /// </summary>
        public MD5(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        internal string CreateMd5(string? password)
        {
            return Md5(password);
        }

        private string Md5(string? password)
        {
            var passwordSalt = _configuration["UserSecrets:PasswordSalt"];

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
                var hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
