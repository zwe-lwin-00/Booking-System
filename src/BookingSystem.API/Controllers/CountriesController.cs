using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(ICountryRepository countryRepository, ILogger<CountriesController> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        _logger.LogInformation("Get all countries requested");
        var countries = await _countryRepository.GetAllAsync();
        var result = countries.Select(c => new
        {
            c.Id,
            c.Name,
            c.Code,
            c.CreatedAt
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> GetById(Guid id)
    {
        _logger.LogInformation("Get country by Id: {CountryId}", id);
        var country = await _countryRepository.GetByIdAsync(id);
        if (country == null)
        {
            _logger.LogWarning("Country not found. CountryId: {CountryId}", id);
            return NotFound();
        }
        return Ok(new
        {
            country.Id,
            country.Name,
            country.Code,
            country.CreatedAt
        });
    }
}
