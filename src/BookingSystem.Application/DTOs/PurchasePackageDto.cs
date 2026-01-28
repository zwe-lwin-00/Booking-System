namespace BookingSystem.Application.DTOs;

public class PurchasePackageDto
{
    public Guid PackageId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
}
