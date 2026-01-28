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

    public CountriesController(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
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
        var country = await _countryRepository.GetByIdAsync(id);
        if (country == null)
            return NotFound();

        return Ok(new
        {
            country.Id,
            country.Name,
            country.Code,
            country.CreatedAt
        });
    }
}
