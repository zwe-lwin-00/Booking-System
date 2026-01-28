using BookingSystem.Domain.Enums;

namespace BookingSystem.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? SpecialRequests { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Room Room { get; set; } = null!;
}
