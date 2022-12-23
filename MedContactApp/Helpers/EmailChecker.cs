using MedContactCore.Abstractions;
using System.Security.Claims;

namespace MedContactApp.Helpers
{
    public class EmailChecker
    {
        private readonly IUserService _userService;
        private readonly IFamilyService _familyService;
        public EmailChecker(IUserService userService, IFamilyService familyService)
        {
            _userService= userService;
            _familyService = familyService;
        }
        public async Task <bool> CheckEmail(string email, HttpContext context)
        {
            var curEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if  (curEmail != null && curEmail.Equals(email)) 
                return true;

            if (!await _userService.CheckUserEmailAsync(email))

                return true;
            else
            {
                var mainUserId = context.User.FindFirst("MUId")?.Value;
                if (Guid.TryParse(mainUserId, out Guid Uid))
                {
                    var family = await _familyService.GetRelativesAsync(Uid);
                    if (family != null && family.Any(r => r.Email != null && r.Email.Equals(email)))
                        return true;
                }
                return false;
            }

        }
    }
}
