using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class UserPackageService : IUserPackageService
{
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly ILogger<UserPackageService> _logger;

    public UserPackageService(IUserPackageRepository userPackageRepository, ILogger<UserPackageService> logger)
    {
        _userPackageRepository = userPackageRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserPackageDto>> GetByUserIdAsync(Guid userId)
    {
        _logger.LogInformation("Get user packages for user {UserId}", userId);
        var userPackages = await _userPackageRepository.GetByUserIdAsync(userId);
        return userPackages.Select(MapToDto);
    }

    public async Task<UserPackageDto?> GetActiveByUserIdAndCountryIdAsync(Guid userId, Guid countryId)
    {
        _logger.LogInformation("Get active user package for user {UserId}, country {CountryId}", userId, countryId);
        var userPackage = await _userPackageRepository.GetActiveByUserIdAndCountryIdAsync(userId, countryId);
        if (userPackage == null)
        {
            _logger.LogWarning("No active user package found for user {UserId}, country {CountryId}", userId, countryId);
            return null;
        }
        return MapToDto(userPackage);
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
