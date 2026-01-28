namespace BookingSystem.Application.DTOs;

public class UpdateBookingDto
{
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? NumberOfGuests { get; set; }
    public string? SpecialRequests { get; set; }
}
