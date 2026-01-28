namespace BookingSystem.Application.DTOs;

public class PackageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int Credits { get; set; }
    public decimal Price { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
}
