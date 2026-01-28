using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Booking>> GetByClassIdAsync(Guid classId);
    Task<IEnumerable<Booking>> GetByUserIdAndTimeRangeAsync(Guid userId, DateTime startTime, DateTime endTime);
    Task<Booking> AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
