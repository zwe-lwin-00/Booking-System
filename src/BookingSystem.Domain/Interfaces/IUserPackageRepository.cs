using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IUserPackageRepository
{
    Task<UserPackage?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserPackage>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserPackage>> GetActiveByUserIdAsync(Guid userId);
    Task<UserPackage?> GetActiveByUserIdAndCountryIdAsync(Guid userId, Guid countryId);
    Task<UserPackage> AddAsync(UserPackage userPackage);
    Task UpdateAsync(UserPackage userPackage);
    Task DeleteAsync(Guid id);
}
