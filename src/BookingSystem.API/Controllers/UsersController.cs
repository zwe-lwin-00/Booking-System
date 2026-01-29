using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Get profile requested for UserId: {UserId}", userId);
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("Profile not found for UserId: {UserId}", userId);
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] CreateUserDto updateUserDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Update profile requested for UserId: {UserId}", userId);
        var user = await _userService.UpdateAsync(userId, updateUserDto);
        _logger.LogInformation("Profile updated successfully for UserId: {UserId}", userId);
        return Ok(user);
    }
}
