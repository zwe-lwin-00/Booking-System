using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IWaitlistRepository
{
    Task<Waitlist?> GetByIdAsync(Guid id);
    Task<IEnumerable<Waitlist>> GetByClassIdAsync(Guid classId);
    Task<IEnumerable<Waitlist>> GetPendingByClassIdAsync(Guid classId);
    Task<Waitlist?> GetNextInWaitlistAsync(Guid classId);
    Task<Waitlist> AddAsync(Waitlist waitlist);
    Task UpdateAsync(Waitlist waitlist);
    Task DeleteAsync(Guid id);
    Task<int> GetNextPositionAsync(Guid classId);
}
