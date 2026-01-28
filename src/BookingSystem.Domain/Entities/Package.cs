namespace BookingSystem.Domain.Entities;

public class Package : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public int Credits { get; set; }
    public decimal Price { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Country Country { get; set; } = null!;
    public virtual ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();
}
