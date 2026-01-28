using BookingSystem.Application.Common.Settings;
using BookingSystem.Application.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace BookingSystem.Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IOptions<ValidationSettings> validationSettings)
    {
        var settings = validationSettings.Value;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(settings.FieldLengths.FirstName)
            .WithMessage($"First name must not exceed {settings.FieldLengths.FirstName} characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(settings.FieldLengths.LastName)
            .WithMessage($"Last name must not exceed {settings.FieldLengths.LastName} characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(settings.FieldLengths.Email)
            .WithMessage($"Email must not exceed {settings.FieldLengths.Email} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}
