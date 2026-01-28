using BookingSystem.Application.Common.Settings;
using BookingSystem.Application.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace BookingSystem.Application.Validators;

public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingDtoValidator(IOptions<ValidationSettings> validationSettings)
    {
        var settings = validationSettings.Value;

        RuleFor(x => x.NumberOfGuests)
            .GreaterThan(0)
            .When(x => x.NumberOfGuests.HasValue)
            .WithMessage("Number of guests must be greater than 0.")
            .LessThanOrEqualTo(settings.MaxGuestsPerBooking)
            .When(x => x.NumberOfGuests.HasValue)
            .WithMessage($"Number of guests cannot exceed {settings.MaxGuestsPerBooking}.");

        RuleFor(x => x)
            .Must(x => !x.CheckInDate.HasValue || !x.CheckOutDate.HasValue || x.CheckInDate < x.CheckOutDate)
            .WithMessage("Check-in date must be before check-out date.");
    }
}
