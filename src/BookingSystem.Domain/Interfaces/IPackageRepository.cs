using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IPackageRepository
{
    Task<Package?> GetByIdAsync(Guid id);
    Task<IEnumerable<Package>> GetAllAsync();
    Task<IEnumerable<Package>> GetByCountryIdAsync(Guid countryId);
    Task<Package> AddAsync(Package package);
    Task UpdateAsync(Package package);
    Task DeleteAsync(Guid id);
}
