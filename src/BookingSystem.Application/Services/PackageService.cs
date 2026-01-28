using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;

namespace BookingSystem.Application.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IPaymentService _paymentService;

    public PackageService(
        IPackageRepository packageRepository,
        IUserPackageRepository userPackageRepository,
        ICountryRepository countryRepository,
        IPaymentService paymentService)
    {
        _packageRepository = packageRepository;
        _userPackageRepository = userPackageRepository;
        _countryRepository = countryRepository;
        _paymentService = paymentService;
    }

    public async Task<PackageDto?> GetByIdAsync(Guid id)
    {
        var package = await _packageRepository.GetByIdAsync(id);
        return package == null ? null : MapToDto(package);
    }

    public async Task<IEnumerable<PackageDto>> GetByCountryIdAsync(Guid countryId)
    {
        var packages = await _packageRepository.GetByCountryIdAsync(countryId);
        return packages.Where(p => p.IsActive).Select(MapToDto);
    }

    public async Task<IEnumerable<PackageDto>> GetAllAsync()
    {
        var packages = await _packageRepository.GetAllAsync();
        return packages.Where(p => p.IsActive).Select(MapToDto);
    }

    public async Task<UserPackageDto> PurchasePackageAsync(Guid userId, PurchasePackageDto purchaseDto)
    {
        var package = await _packageRepository.GetByIdAsync(purchaseDto.PackageId);
        if (package == null)
            throw new NotFoundException(nameof(Package), purchaseDto.PackageId);

        if (!package.IsActive)
            throw new InvalidOperationException("Package is not available for purchase");

        // Process payment (mock)
        var paymentSuccess = _paymentService.PaymentCharge(
            package.Price,
            purchaseDto.CardNumber,
            purchaseDto.CardHolderName,
            purchaseDto.ExpiryDate,
            purchaseDto.CVV);

        if (!paymentSuccess)
            throw new InvalidOperationException("Payment processing failed");

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
