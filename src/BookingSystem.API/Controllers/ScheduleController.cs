using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpPost("book")]
    public async Task<ActionResult<BookingDto>> BookClass([FromBody] BookClassDto bookClassDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var booking = await _scheduleService.BookClassAsync(userId, bookClassDto);
        return Ok(booking);
    }

    [HttpPost("cancel/{bookingId}")]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _scheduleService.CancelBookingAsync(userId, bookingId);
        return Ok(new { message = "Booking cancelled successfully" });
    }

    [HttpPost("waitlist")]
    public async Task<IActionResult> AddToWaitlist([FromBody] BookClassDto bookClassDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _scheduleService.AddToWaitlistAsync(userId, bookClassDto.ClassId, bookClassDto.UserPackageId);
        return Ok(new { message = "Added to waitlist successfully" });
    }

    [HttpPost("check-in/{bookingId}")]
    public async Task<IActionResult> CheckIn(Guid bookingId)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _scheduleService.CheckInAsync(userId, bookingId);
        return Ok(new { message = "Checked in successfully" });
    }

    [HttpGet("my-bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var bookings = await _scheduleService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }
}
