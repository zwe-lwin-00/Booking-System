namespace BookingSystem.Application.Services;

public interface IEmailService
{
    bool SendVerifyEmail(string email, string token);
    bool SendPasswordResetEmail(string email, string token);
}
