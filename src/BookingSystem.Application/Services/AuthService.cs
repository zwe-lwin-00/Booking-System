using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
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

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        if (registerDto.Password != registerDto.ConfirmPassword)
            throw new InvalidOperationException("Passwords do not match");

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

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = MapToUserDto(createdUser)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash, false))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!user.IsEmailVerified)
            throw new UnauthorizedAccessException("Email not verified. Please verify your email first.");

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = MapToUserDto(user)
        };
    }

    public async Task VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
    {
        var user = await _userRepository.GetByEmailAsync(verifyEmailDto.Email);
        if (user == null)
            throw new NotFoundException(nameof(User), verifyEmailDto.Email);

        if (user.IsEmailVerified)
            throw new InvalidOperationException("Email already verified");

        if (user.EmailVerificationToken != verifyEmailDto.Token ||
            user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid or expired verification token");

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;
        await _userRepository.UpdateAsync(user);
    }

    public async Task SendVerificationEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new NotFoundException(nameof(User), email);

        if (user.IsEmailVerified)
            throw new InvalidOperationException("Email already verified");

        var verificationToken = Guid.NewGuid().ToString();
        user.EmailVerificationToken = verificationToken;
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _userRepository.UpdateAsync(user);

        _emailService.SendVerifyEmail(email, verificationToken);
    }

    public async Task SendPasswordResetEmailAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
        if (user == null)
            return; // Don't reveal if user exists

        var resetToken = Guid.NewGuid().ToString();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateAsync(user);

        _emailService.SendPasswordResetEmail(user.Email, resetToken);
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.PasswordResetToken == token &&
                                            u.PasswordResetTokenExpiry > DateTime.UtcNow);

        if (user == null)
            throw new InvalidOperationException("Invalid or expired reset token");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, BCrypt.Net.BCrypt.GenerateSalt());
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _userRepository.UpdateAsync(user);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            throw new InvalidOperationException("New passwords do not match");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash, false))
            throw new UnauthorizedAccessException("Current password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword, BCrypt.Net.BCrypt.GenerateSalt());
        await _userRepository.UpdateAsync(user);
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
