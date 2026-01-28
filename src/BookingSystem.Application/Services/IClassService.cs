using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IClassService
{
    Task<ClassDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ClassDto>> GetByCountryIdAsync(Guid countryId);
    Task<IEnumerable<ClassDto>> GetUpcomingAsync();
}
