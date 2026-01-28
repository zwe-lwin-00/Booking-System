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

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetAll()
    {
        var classes = await _classService.GetUpcomingAsync();
        return Ok(classes);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ClassDto>> GetById(Guid id)
    {
        var classEntity = await _classService.GetByIdAsync(id);
        if (classEntity == null)
            return NotFound();

        return Ok(classEntity);
    }

    [HttpGet("country/{countryId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetByCountry(Guid countryId)
    {
        var classes = await _classService.GetByCountryIdAsync(countryId);
        return Ok(classes);
    }
}
