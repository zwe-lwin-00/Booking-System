using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PackageService> _logger;

    public PackageService(
        IPackageRepository packageRepository,
        IUserPackageRepository userPackageRepository,
        ICountryRepository countryRepository,
        IPaymentService paymentService,
        ILogger<PackageService> logger)
    {
        _packageRepository = packageRepository;
        _userPackageRepository = userPackageRepository;
        _countryRepository = countryRepository;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<PackageDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Get package by id {PackageId}", id);
        var package = await _packageRepository.GetByIdAsync(id);
        if (package == null)
        {
            _logger.LogWarning("Package {PackageId} not found", id);
            return null;
        }
        return MapToDto(package);
    }

    public async Task<IEnumerable<PackageDto>> GetByCountryIdAsync(Guid countryId)
    {
        _logger.LogInformation("Get packages by country {CountryId}", countryId);
        var packages = await _packageRepository.GetByCountryIdAsync(countryId);
        return packages.Where(p => p.IsActive).Select(MapToDto);
    }

    public async Task<IEnumerable<PackageDto>> GetAllAsync()
    {
        _logger.LogInformation("Get all packages");
        var packages = await _packageRepository.GetAllAsync();
        return packages.Where(p => p.IsActive).Select(MapToDto);
    }

    public async Task<UserPackageDto> PurchasePackageAsync(Guid userId, PurchasePackageDto purchaseDto)
    {
        _logger.LogInformation("Purchase package {PackageId} by user {UserId}", purchaseDto.PackageId, userId);

        var package = await _packageRepository.GetByIdAsync(purchaseDto.PackageId);
        if (package == null)
        {
            _logger.LogWarning("Purchase failed: package {PackageId} not found", purchaseDto.PackageId);
            throw new NotFoundException(nameof(Package), purchaseDto.PackageId);
        }

        if (!package.IsActive)
        {
            _logger.LogWarning("Purchase failed: package {PackageId} is not active", purchaseDto.PackageId);
            throw new InvalidOperationException("Package is not available for purchase");
        }

        // Process payment (mock)
        var paymentSuccess = _paymentService.PaymentCharge(
            package.Price,
            purchaseDto.CardNumber,
            purchaseDto.CardHolderName,
            purchaseDto.ExpiryDate,
            purchaseDto.CVV);

        if (!paymentSuccess)
        {
            _logger.LogError("Purchase failed for user {UserId}, package {PackageId}: payment processing failed", userId, purchaseDto.PackageId);
            throw new InvalidOperationException("Payment processing failed");
        }

        // Create user package
        var userPackage = new UserPackage
        {
            UserId = userId,
            PackageId = package.Id,
            RemainingCredits = package.Credits,
            PurchaseDate = DateTime.UtcNow,
            ExpiryDate = package.ExpiryDate
        };

        var createdUserPackage = await _userPackageRepository.AddAsync(userPackage);
        _logger.LogInformation("Package purchased successfully. UserPackageId: {UserPackageId}, UserId: {UserId}, PackageId: {PackageId}", createdUserPackage.Id, userId, purchaseDto.PackageId);
        return MapToUserPackageDto(createdUserPackage);
    }

    private static PackageDto MapToDto(Package package)
    {
        return new PackageDto
        {
            Id = package.Id,
            Name = package.Name,
            CountryId = package.CountryId,
            CountryName = package.Country.Name,
            CountryCode = package.Country.Code,
            Credits = package.Credits,
            Price = package.Price,
            ExpiryDate = package.ExpiryDate,
            IsActive = package.IsActive
        };
    }

    private static UserPackageDto MapToUserPackageDto(UserPackage userPackage)
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
