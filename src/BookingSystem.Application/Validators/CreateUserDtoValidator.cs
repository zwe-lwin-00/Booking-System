using BookingSystem.Application.Common.Settings;
using BookingSystem.Application.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace BookingSystem.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator(IOptions<ValidationSettings> validationSettings)
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

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(new Regex(settings.PhoneValidation.Pattern))
            .WithMessage(settings.PhoneValidation.ErrorMessage);
    }
}
