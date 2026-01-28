using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserDto>> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
    {
        var user = await _userService.CreateAsync(createUserDto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] CreateUserDto updateUserDto)
    {
        var user = await _userService.UpdateAsync(id, updateUserDto);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}
