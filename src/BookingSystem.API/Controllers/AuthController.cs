using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);
        var result = await _authService.RegisterAsync(registerDto);
        _logger.LogInformation("User registered successfully. UserId: {UserId}", result.User.Id);
        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);
        var result = await _authService.LoginAsync(loginDto);
        _logger.LogInformation("User logged in successfully. UserId: {UserId}", result.User.Id);
        return Ok(result);
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
    {
        _logger.LogInformation("Email verification attempt for: {Email}", verifyEmailDto.Email);
        await _authService.VerifyEmailAsync(verifyEmailDto);
        _logger.LogInformation("Email verified successfully for: {Email}", verifyEmailDto.Email);
        return Ok(new { message = "Email verified successfully" });
    }

    [HttpPost("resend-verification")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendVerification([FromBody] string email)
    {
        _logger.LogInformation("Resend verification email requested for: {Email}", email);
        await _authService.SendVerificationEmailAsync(email);
        _logger.LogInformation("Verification email sent for: {Email}", email);
        return Ok(new { message = "Verification email sent" });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        _logger.LogInformation("Forgot password requested for: {Email}", resetPasswordDto.Email);
        await _authService.SendPasswordResetEmailAsync(resetPasswordDto);
        _logger.LogInformation("Password reset email sent if account exists for: {Email}", resetPasswordDto.Email);
        return Ok(new { message = "Password reset email sent if account exists" });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] string newPassword)
    {
        _logger.LogInformation("Reset password attempt with token");
        await _authService.ResetPasswordAsync(token, newPassword);
        _logger.LogInformation("Password reset successfully");
        return Ok(new { message = "Password reset successfully" });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Change password requested for UserId: {UserId}", userId);
        await _authService.ChangePasswordAsync(userId, changePasswordDto);
        _logger.LogInformation("Password changed successfully for UserId: {UserId}", userId);
        return Ok(new { message = "Password changed successfully" });
    }
}
