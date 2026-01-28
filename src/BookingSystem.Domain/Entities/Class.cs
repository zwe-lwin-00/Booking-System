namespace BookingSystem.Domain.Entities;

public class Class : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RequiredCredits { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentBookings { get; set; } = 0;
    public bool IsFull => CurrentBookings >= MaxCapacity;
    
    // Navigation properties
    public virtual Country Country { get; set; } = null!;
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
