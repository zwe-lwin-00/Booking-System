using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IPackageService
{
    Task<PackageDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PackageDto>> GetByCountryIdAsync(Guid countryId);
    Task<IEnumerable<PackageDto>> GetAllAsync();
    Task<UserPackageDto> PurchasePackageAsync(Guid userId, PurchasePackageDto purchaseDto);
}
