using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IScheduleService
{
    Task<BookingDto> BookClassAsync(Guid userId, BookClassDto bookClassDto);
    Task CancelBookingAsync(Guid userId, Guid bookingId);
    Task AddToWaitlistAsync(Guid userId, Guid classId, Guid userPackageId);
    Task CheckInAsync(Guid userId, Guid bookingId);
    Task<IEnumerable<BookingDto>> GetUserBookingsAsync(Guid userId);
}
