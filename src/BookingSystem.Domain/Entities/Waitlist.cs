namespace BookingSystem.Domain.Entities;

public class Waitlist : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ClassId { get; set; }
    public Guid UserPackageId { get; set; }
    public int CreditsReserved { get; set; }
    public int Position { get; set; } // FIFO order
    public bool IsPromoted { get; set; } = false; // Promoted to booking
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
    public virtual UserPackage UserPackage { get; set; } = null!;
}
