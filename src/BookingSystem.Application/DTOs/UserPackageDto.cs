namespace BookingSystem.Application.DTOs;

public class UserPackageDto
{
    public Guid Id { get; set; }
    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int RemainingCredits { get; set; }
    public int TotalCredits { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
}
