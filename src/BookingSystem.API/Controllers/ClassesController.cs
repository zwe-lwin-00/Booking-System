using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;
    private readonly ILogger<ClassesController> _logger;

    public ClassesController(IClassService classService, ILogger<ClassesController> logger)
    {
        _classService = classService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetAll()
    {
        _logger.LogInformation("Get all upcoming classes requested");
        var classes = await _classService.GetUpcomingAsync();
        return Ok(classes);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ClassDto>> GetById(Guid id)
    {
        _logger.LogInformation("Get class by Id: {ClassId}", id);
        var classEntity = await _classService.GetByIdAsync(id);
        if (classEntity == null)
        {
            _logger.LogWarning("Class not found. ClassId: {ClassId}", id);
            return NotFound();
        }
        return Ok(classEntity);
    }

    [HttpGet("country/{countryId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetByCountry(Guid countryId)
    {
        _logger.LogInformation("Get classes by country: {CountryId}", countryId);
        var classes = await _classService.GetByCountryIdAsync(countryId);
        return Ok(classes);
    }
}
