using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IClassRepository
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<IEnumerable<Class>> GetAllAsync();
    Task<IEnumerable<Class>> GetByCountryIdAsync(Guid countryId);
    Task<IEnumerable<Class>> GetUpcomingAsync(DateTime fromDate);
    Task<Class> AddAsync(Class classEntity);
    Task UpdateAsync(Class classEntity);
    Task DeleteAsync(Guid id);
    Task IncrementBookingCountAsync(Guid classId);
    Task DecrementBookingCountAsync(Guid classId);
}
