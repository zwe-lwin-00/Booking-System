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

    public PackagesController(IPackageService packageService, IUserPackageService userPackageService)
    {
        _packageService = packageService;
        _userPackageService = userPackageService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PackageDto>>> GetAll()
    {
        var packages = await _packageService.GetAllAsync();
        return Ok(packages);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PackageDto>> GetById(Guid id)
    {
        var package = await _packageService.GetByIdAsync(id);
        if (package == null)
            return NotFound();

        return Ok(package);
    }

    [HttpGet("country/{countryId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PackageDto>>> GetByCountry(Guid countryId)
    {
        var packages = await _packageService.GetByCountryIdAsync(countryId);
        return Ok(packages);
    }

    [HttpPost("purchase")]
    public async Task<ActionResult<UserPackageDto>> Purchase([FromBody] PurchasePackageDto purchaseDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var userPackage = await _packageService.PurchasePackageAsync(userId, purchaseDto);
        return Ok(userPackage);
    }

    [HttpGet("my-packages")]
    public async Task<ActionResult<IEnumerable<UserPackageDto>>> GetMyPackages()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var packages = await _userPackageService.GetByUserIdAsync(userId);
        return Ok(packages);
    }
}
