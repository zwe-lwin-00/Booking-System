namespace BookingSystem.Application.DTOs;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RequiredCredits { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentBookings { get; set; }
    public int AvailableSlots { get; set; }
    public bool IsFull { get; set; }
}
