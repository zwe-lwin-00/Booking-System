namespace BookingSystem.Application.Services;

public class EmailService : IEmailService
{
    public bool SendVerifyEmail(string email, string token)
    {
        // Mock email service - in production, integrate with real email service
        // For now, just return true to simulate success
        // In real implementation: Send email with verification link containing token
        return true;
    }

    public bool SendPasswordResetEmail(string email, string token)
    {
        // Mock email service
        // In real implementation: Send email with password reset link containing token
        return true;
    }
}
