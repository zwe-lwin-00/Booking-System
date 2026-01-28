using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IUserPackageService
{
    Task<IEnumerable<UserPackageDto>> GetByUserIdAsync(Guid userId);
    Task<UserPackageDto?> GetActiveByUserIdAndCountryIdAsync(Guid userId, Guid countryId);
}
