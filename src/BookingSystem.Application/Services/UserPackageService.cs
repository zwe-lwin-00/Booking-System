using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Interfaces;

namespace BookingSystem.Application.Services;

public class UserPackageService : IUserPackageService
{
    private readonly IUserPackageRepository _userPackageRepository;

    public UserPackageService(IUserPackageRepository userPackageRepository)
    {
        _userPackageRepository = userPackageRepository;
    }

    public async Task<IEnumerable<UserPackageDto>> GetByUserIdAsync(Guid userId)
    {
        var userPackages = await _userPackageRepository.GetByUserIdAsync(userId);
        return userPackages.Select(MapToDto);
    }

    public async Task<UserPackageDto?> GetActiveByUserIdAndCountryIdAsync(Guid userId, Guid countryId)
    {
        var userPackage = await _userPackageRepository.GetActiveByUserIdAndCountryIdAsync(userId, countryId);
        return userPackage == null ? null : MapToDto(userPackage);
    }

    private static UserPackageDto MapToDto(Domain.Entities.UserPackage userPackage)
    {
        return new UserPackageDto
        {
            Id = userPackage.Id,
            PackageId = userPackage.PackageId,
            PackageName = userPackage.Package.Name,
            CountryName = userPackage.Package.Country.Name,
            CountryCode = userPackage.Package.Country.Code,
            RemainingCredits = userPackage.RemainingCredits,
            TotalCredits = userPackage.Package.Credits,
            PurchaseDate = userPackage.PurchaseDate,
            ExpiryDate = userPackage.ExpiryDate,
            IsExpired = userPackage.IsExpired
        };
    }
}
