namespace BookingSystem.Domain.Entities;

public class Room : BaseEntity
{
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
