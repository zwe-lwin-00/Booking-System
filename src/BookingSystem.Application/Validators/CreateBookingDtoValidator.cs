using BookingSystem.Application.DTOs;
using FluentValidation;

namespace BookingSystem.Application.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Room ID is required.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty().WithMessage("Check-in date is required.")
            .Must(BeInFuture).WithMessage("Check-in date must be in the future.")
            .LessThan(x => x.CheckOutDate).WithMessage("Check-in date must be before check-out date.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty().WithMessage("Check-out date is required.")
            .GreaterThan(x => x.CheckInDate).WithMessage("Check-out date must be after check-in date.");

        RuleFor(x => x.NumberOfGuests)
            .GreaterThan(0).WithMessage("Number of guests must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Number of guests cannot exceed 10.");
    }

    private bool BeInFuture(DateTime date)
    {
        return date >= DateTime.Today;
    }
}
