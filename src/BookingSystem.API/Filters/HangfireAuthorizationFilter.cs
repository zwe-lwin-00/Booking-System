using Hangfire.Dashboard;

namespace BookingSystem.API.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In production, implement proper authorization
        // For now, allow access in development
        return true;
    }
}
