using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        _logger.LogInformation("Registration attempt for email {Email}", registerDto.Email);

        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: user with email {Email} already exists", registerDto.Email);
            throw new InvalidOperationException("User with this email already exists");
        }

        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            _logger.LogWarning("Registration failed for {Email}: passwords do not match", registerDto.Email);
            throw new InvalidOperationException("Passwords do not match");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, BCrypt.Net.BCrypt.GenerateSalt());

        // Generate email verification token
        var verificationToken = Guid.NewGuid().ToString();

        var user = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            PasswordHash = passwordHash,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1)
        };

        var createdUser = await _userRepository.AddAsync(user);

        // Send verification email (mock)
        _emailService.SendVerifyEmail(createdUser.Email, verificationToken);

        // Generate JWT token
        var token = GenerateJwtToken(createdUser);

        _logger.LogInformation("User registered successfully. UserId: {UserId}, Email: {Email}", createdUser.Id, createdUser.Email);
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = MapToUserDto(createdUser)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for email {Email}", loginDto.Email);

        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash, false))
        {
            _logger.LogWarning("Login failed for email {Email}: invalid credentials", loginDto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsEmailVerified)
        {
            _logger.LogWarning("Login failed for email {Email}: email not verified", loginDto.Email);
            throw new UnauthorizedAccessException("Email not verified. Please verify your email first.");
        }

        var token = GenerateJwtToken(user);
        _logger.LogInformation("User logged in successfully. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = MapToUserDto(user)
        };
    }

    public async Task VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
    {
        _logger.LogInformation("Email verification attempt for {Email}", verifyEmailDto.Email);

        var user = await _userRepository.GetByEmailAsync(verifyEmailDto.Email);
        if (user == null)
        {
            _logger.LogWarning("Email verification failed: user {Email} not found", verifyEmailDto.Email);
            throw new NotFoundException(nameof(User), verifyEmailDto.Email);
        }

        if (user.IsEmailVerified)
        {
            _logger.LogWarning("Email verification failed for {Email}: already verified", verifyEmailDto.Email);
            throw new InvalidOperationException("Email already verified");
        }

        if (user.EmailVerificationToken != verifyEmailDto.Token ||
            user.EmailVerificationTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogWarning("Email verification failed for {Email}: invalid or expired token", verifyEmailDto.Email);
            throw new InvalidOperationException("Invalid or expired verification token");
        }

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Email verified successfully for user {UserId}, {Email}", user.Id, user.Email);
    }

    public async Task SendVerificationEmailAsync(string email)
    {
        _logger.LogInformation("Send verification email requested for {Email}", email);

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Send verification email failed: user {Email} not found", email);
            throw new NotFoundException(nameof(User), email);
        }

        if (user.IsEmailVerified)
        {
            _logger.LogWarning("Send verification email failed for {Email}: already verified", email);
            throw new InvalidOperationException("Email already verified");
        }

        var verificationToken = Guid.NewGuid().ToString();
        user.EmailVerificationToken = verificationToken;
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _userRepository.UpdateAsync(user);

        _emailService.SendVerifyEmail(email, verificationToken);
        _logger.LogInformation("Verification email sent for user {UserId}, {Email}", user.Id, email);
    }

    public async Task SendPasswordResetEmailAsync(ResetPasswordDto resetPasswordDto)
    {
        _logger.LogInformation("Password reset email requested for {Email}", resetPasswordDto.Email);

        var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            _logger.LogWarning("Password reset requested for non-existent email {Email} (silent skip)", resetPasswordDto.Email);
            return; // Don't reveal if user exists
        }

        var resetToken = Guid.NewGuid().ToString();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateAsync(user);

        _emailService.SendPasswordResetEmail(user.Email, resetToken);
        _logger.LogInformation("Password reset email sent for user {UserId}", user.Id);
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        _logger.LogInformation("Password reset attempt with token");

        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.PasswordResetToken == token &&
                                            u.PasswordResetTokenExpiry > DateTime.UtcNow);

        if (user == null)
        {
            _logger.LogWarning("Password reset failed: invalid or expired token");
            throw new InvalidOperationException("Invalid or expired reset token");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, BCrypt.Net.BCrypt.GenerateSalt());
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        _logger.LogInformation("Change password requested for user {UserId}", userId);

        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
        {
            _logger.LogWarning("Change password failed for user {UserId}: new passwords do not match", userId);
            throw new InvalidOperationException("New passwords do not match");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("Change password failed: user {UserId} not found", userId);
            throw new NotFoundException(nameof(User), userId);
        }

        if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash, false))
        {
            _logger.LogWarning("Change password failed for user {UserId}: current password incorrect", userId);
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword, BCrypt.Net.BCrypt.GenerateSalt());
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Password changed successfully for user {UserId}", userId);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "BookingSystem",
            audience: _configuration["Jwt:Audience"] ?? "BookingSystem",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}
