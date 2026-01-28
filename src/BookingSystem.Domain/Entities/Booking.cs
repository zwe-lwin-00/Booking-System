using BookingSystem.Domain.Enums;

namespace BookingSystem.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ClassId { get; set; }
    public Guid UserPackageId { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Booked;
    public int CreditsUsed { get; set; }
    public bool IsCheckedIn { get; set; } = false;
    public DateTime? CheckInTime { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
    public virtual UserPackage UserPackage { get; set; } = null!;
}
