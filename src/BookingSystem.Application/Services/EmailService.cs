using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public bool SendVerifyEmail(string email, string token)
    {
        _logger.LogInformation("Send verification email to {Email} (mock)", email);
        // Mock email service - in production, integrate with real email service
        // For now, just return true to simulate success
        // In real implementation: Send email with verification link containing token
        return true;
    }

    public bool SendPasswordResetEmail(string email, string token)
    {
        _logger.LogInformation("Send password reset email to {Email} (mock)", email);
        // Mock email service
        // In real implementation: Send email with password reset link containing token
        return true;
    }
}
