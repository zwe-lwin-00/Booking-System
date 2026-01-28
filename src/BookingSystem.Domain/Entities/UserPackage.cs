namespace BookingSystem.Domain.Entities;

public class UserPackage : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public int RemainingCredits { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Package Package { get; set; } = null!;
}
