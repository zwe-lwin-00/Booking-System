using BookingSystem.Application.Common.Settings;
using BookingSystem.Application.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace BookingSystem.Application.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator(IOptions<ValidationSettings> validationSettings)
    {
        var settings = validationSettings.Value;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Room ID is required.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty().WithMessage("Check-in date is required.")
            .Must(date => BeValidCheckInDate(date, settings))
            .WithMessage(GetCheckInDateMessage(settings))
            .LessThan(x => x.CheckOutDate).WithMessage("Check-in date must be before check-out date.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty().WithMessage("Check-out date is required.")
            .GreaterThan(x => x.CheckInDate).WithMessage("Check-out date must be after check-in date.")
            .Must((dto, checkOut) => BeValidDateRange(dto.CheckInDate, checkOut, settings))
            .WithMessage($"Check-out date cannot be more than {settings.MaxBookingDaysInAdvance} days in advance.");

        RuleFor(x => x.NumberOfGuests)
            .GreaterThan(0).WithMessage("Number of guests must be greater than 0.")
            .LessThanOrEqualTo(settings.MaxGuestsPerBooking)
            .WithMessage($"Number of guests cannot exceed {settings.MaxGuestsPerBooking}.");
    }

    private static bool BeValidCheckInDate(DateTime date, ValidationSettings settings)
    {
        var today = DateTime.Today;
        if (settings.AllowSameDayCheckIn)
        {
            return date >= today;
        }
        return date > today;
    }

    private static string GetCheckInDateMessage(ValidationSettings settings)
    {
        return settings.AllowSameDayCheckIn
            ? "Check-in date must be today or in the future."
            : "Check-in date must be in the future.";
    }

    private static bool BeValidDateRange(DateTime checkIn, DateTime checkOut, ValidationSettings settings)
    {
        var daysDifference = (checkOut - checkIn).Days;
        return daysDifference <= settings.MaxBookingDaysInAdvance;
    }
}
