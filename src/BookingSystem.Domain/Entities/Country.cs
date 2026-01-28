namespace BookingSystem.Domain.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // e.g., "SG", "MM"
    
    // Navigation properties
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
