using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using BookingSystem.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
    {
        var bookings = await _bookingService.GetAllAsync();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(Guid id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByUserId(Guid userId)
    {
        var bookings = await _bookingService.GetByUserIdAsync(userId);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto createBookingDto)
    {
        var booking = await _bookingService.CreateAsync(createBookingDto);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<BookingDto>> UpdateStatus(Guid id, [FromBody] BookingStatus status)
    {
        var booking = await _bookingService.UpdateStatusAsync(id, status);
        return Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _bookingService.DeleteAsync(id);
        return NoContent();
    }
}
