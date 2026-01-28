using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface ICountryRepository
{
    Task<Country?> GetByIdAsync(Guid id);
    Task<Country?> GetByCodeAsync(string code);
    Task<IEnumerable<Country>> GetAllAsync();
    Task<Country> AddAsync(Country country);
    Task UpdateAsync(Country country);
    Task DeleteAsync(Guid id);
}
