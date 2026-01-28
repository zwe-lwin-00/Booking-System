using BookingSystem.Application.DTOs;
using FluentValidation;

namespace BookingSystem.Application.Validators;

public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator()
    {
        RuleFor(x => x.RoomNumber)
            .NotEmpty().WithMessage("Room number is required.")
            .MaximumLength(50).WithMessage("Room number must not exceed 50 characters.");

        RuleFor(x => x.RoomType)
            .NotEmpty().WithMessage("Room type is required.")
            .MaximumLength(100).WithMessage("Room type must not exceed 100 characters.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Capacity cannot exceed 10.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
