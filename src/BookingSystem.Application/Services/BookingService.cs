using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Interfaces;
using FluentValidation;

namespace BookingSystem.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateBookingDto> _validator;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IUserRepository userRepository,
        IValidator<CreateBookingDto> validator)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        return booking == null ? null : MapToDto(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return bookings.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetByUserIdAsync(Guid userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);
        return bookings.Select(MapToDto);
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto createBookingDto)
    {
        // Validate input
        var validationResult = await _validator.ValidateAsync(createBookingDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Validate room exists
        var room = await _roomRepository.GetByIdAsync(createBookingDto.RoomId);
        if (room == null)
            throw new NotFoundException(nameof(Room), createBookingDto.RoomId);

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(createBookingDto.UserId);
        if (user == null)
            throw new NotFoundException(nameof(User), createBookingDto.UserId);

        // Check if room is available for the dates
        var conflictingBookings = await _bookingRepository.GetByDateRangeAsync(
            createBookingDto.CheckInDate, createBookingDto.CheckOutDate);
        
        if (conflictingBookings.Any(b => b.RoomId == createBookingDto.RoomId && 
                                         b.Status == BookingStatus.Confirmed))
        {
            throw new InvalidOperationException("Room is not available for the selected dates");
        }

        // Calculate total price
        var nights = (createBookingDto.CheckOutDate - createBookingDto.CheckInDate).Days;
        var totalPrice = room.PricePerNight * nights;

        var booking = new Booking
        {
            UserId = createBookingDto.UserId,
            RoomId = createBookingDto.RoomId,
            CheckInDate = createBookingDto.CheckInDate,
            CheckOutDate = createBookingDto.CheckOutDate,
            NumberOfGuests = createBookingDto.NumberOfGuests,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            SpecialRequests = createBookingDto.SpecialRequests
        };

        var createdBooking = await _bookingRepository.AddAsync(booking);
        return MapToDto(createdBooking);
    }

    public async Task<BookingDto> UpdateStatusAsync(Guid id, BookingStatus status)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), id);

        booking.Status = status;
        await _bookingRepository.UpdateAsync(booking);
        return MapToDto(booking);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _bookingRepository.DeleteAsync(id);
    }

    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            UserName = $"{booking.User.FirstName} {booking.User.LastName}",
            RoomId = booking.RoomId,
            RoomNumber = booking.Room.RoomNumber,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            NumberOfGuests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            SpecialRequests = booking.SpecialRequests,
            CreatedAt = booking.CreatedAt
        };
    }
}
