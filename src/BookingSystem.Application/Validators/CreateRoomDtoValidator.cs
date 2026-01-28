using BookingSystem.Application.Common.Settings;
using BookingSystem.Application.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace BookingSystem.Application.Validators;

public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator(IOptions<ValidationSettings> validationSettings)
    {
        var settings = validationSettings.Value;

        RuleFor(x => x.RoomNumber)
            .NotEmpty().WithMessage("Room number is required.")
            .MaximumLength(settings.FieldLengths.RoomNumber)
            .WithMessage($"Room number must not exceed {settings.FieldLengths.RoomNumber} characters.");

        RuleFor(x => x.RoomType)
            .NotEmpty().WithMessage("Room type is required.")
            .MaximumLength(settings.FieldLengths.RoomType)
            .WithMessage($"Room type must not exceed {settings.FieldLengths.RoomType} characters.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.")
            .LessThanOrEqualTo(settings.MaxRoomCapacity)
            .WithMessage($"Capacity cannot exceed {settings.MaxRoomCapacity}.");

        RuleFor(x => x.Description)
            .MaximumLength(settings.FieldLengths.RoomDescription)
            .WithMessage($"Description must not exceed {settings.FieldLengths.RoomDescription} characters.");
    }
}
