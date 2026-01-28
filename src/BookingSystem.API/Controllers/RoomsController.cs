using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
    {
        var rooms = await _roomService.GetAllAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(Guid id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailable(
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut)
    {
        var rooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut);
        return Ok(rooms);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto createRoomDto)
    {
        var room = await _roomService.CreateAsync(createRoomDto);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomDto>> Update(Guid id, [FromBody] CreateRoomDto updateRoomDto)
    {
        var room = await _roomService.UpdateAsync(id, updateRoomDto);
        return Ok(room);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roomService.DeleteAsync(id);
        return NoContent();
    }
}
