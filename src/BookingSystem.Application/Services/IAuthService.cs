using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task VerifyEmailAsync(VerifyEmailDto verifyEmailDto);
    Task SendVerificationEmailAsync(string email);
    Task SendPasswordResetEmailAsync(ResetPasswordDto resetPasswordDto);
    Task ResetPasswordAsync(string token, string newPassword);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
}
