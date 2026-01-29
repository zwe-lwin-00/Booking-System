using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;
    private readonly IUserPackageService _userPackageService;
    private readonly ILogger<PackagesController> _logger;

    public PackagesController(IPackageService packageService, IUserPackageService userPackageService, ILogger<PackagesController> logger)
    {
        _packageService = packageService;
        _userPackageService = userPackageService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PackageDto>>> GetAll()
    {
        _logger.LogInformation("Get all packages requested");
        var packages = await _packageService.GetAllAsync();
        return Ok(packages);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PackageDto>> GetById(Guid id)
    {
        _logger.LogInformation("Get package by Id: {PackageId}", id);
        var package = await _packageService.GetByIdAsync(id);
        if (package == null)
        {
            _logger.LogWarning("Package not found. PackageId: {PackageId}", id);
            return NotFound();
        }
        return Ok(package);
    }

    [HttpGet("country/{countryId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PackageDto>>> GetByCountry(Guid countryId)
    {
        _logger.LogInformation("Get packages by country: {CountryId}", countryId);
        var packages = await _packageService.GetByCountryIdAsync(countryId);
        return Ok(packages);
    }

    [HttpPost("purchase")]
    public async Task<ActionResult<UserPackageDto>> Purchase([FromBody] PurchasePackageDto purchaseDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Package purchase requested. UserId: {UserId}, PackageId: {PackageId}", userId, purchaseDto.PackageId);
        var userPackage = await _packageService.PurchasePackageAsync(userId, purchaseDto);
        _logger.LogInformation("Package purchased successfully. UserId: {UserId}, UserPackageId: {UserPackageId}", userId, userPackage.Id);
        return Ok(userPackage);
    }

    [HttpGet("my-packages")]
    public async Task<ActionResult<IEnumerable<UserPackageDto>>> GetMyPackages()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Get my packages requested for UserId: {UserId}", userId);
        var packages = await _userPackageService.GetByUserIdAsync(userId);
        return Ok(packages);
    }
}
