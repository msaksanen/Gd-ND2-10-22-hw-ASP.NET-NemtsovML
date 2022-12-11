using Hangfire.Dashboard;
using MedContactDb.Entities;

namespace MedContactApp.Filters
{
    public class HangfireDashBoardFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext Context)
        {
            var context = Context.GetHttpContext();

            if (context.User.Identities.Any(identity => identity.IsAuthenticated) &&
                context.User.IsInRole("Admin"))
                return true;
            else
                return false;
        }
    }
}
