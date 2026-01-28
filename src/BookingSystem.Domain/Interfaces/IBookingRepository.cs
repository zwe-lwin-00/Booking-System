using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime checkIn, DateTime checkOut);
    Task<Booking> AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
