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
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(IScheduleService scheduleService, ILogger<ScheduleController> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
    }

    [HttpPost("book")]
    public async Task<ActionResult<BookingDto>> BookClass([FromBody] BookClassDto bookClassDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Book class requested. UserId: {UserId}, ClassId: {ClassId}", userId, bookClassDto.ClassId);
        var booking = await _scheduleService.BookClassAsync(userId, bookClassDto);
        _logger.LogInformation("Class booked successfully. UserId: {UserId}, BookingId: {BookingId}", userId, booking.Id);
        return Ok(booking);
    }

    [HttpPost("cancel/{bookingId}")]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Cancel booking requested. UserId: {UserId}, BookingId: {BookingId}", userId, bookingId);
        await _scheduleService.CancelBookingAsync(userId, bookingId);
        _logger.LogInformation("Booking cancelled successfully. UserId: {UserId}, BookingId: {BookingId}", userId, bookingId);
        return Ok(new { message = "Booking cancelled successfully" });
    }

    [HttpPost("waitlist")]
    public async Task<IActionResult> AddToWaitlist([FromBody] BookClassDto bookClassDto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Add to waitlist requested. UserId: {UserId}, ClassId: {ClassId}", userId, bookClassDto.ClassId);
        await _scheduleService.AddToWaitlistAsync(userId, bookClassDto.ClassId, bookClassDto.UserPackageId);
        _logger.LogInformation("Added to waitlist successfully. UserId: {UserId}, ClassId: {ClassId}", userId, bookClassDto.ClassId);
        return Ok(new { message = "Added to waitlist successfully" });
    }

    [HttpPost("check-in/{bookingId}")]
    public async Task<IActionResult> CheckIn(Guid bookingId)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Check-in requested. UserId: {UserId}, BookingId: {BookingId}", userId, bookingId);
        await _scheduleService.CheckInAsync(userId, bookingId);
        _logger.LogInformation("Check-in successful. UserId: {UserId}, BookingId: {BookingId}", userId, bookingId);
        return Ok(new { message = "Checked in successfully" });
    }

    [HttpGet("my-bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _logger.LogInformation("Get my bookings requested for UserId: {UserId}", userId);
        var bookings = await _scheduleService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }
}
