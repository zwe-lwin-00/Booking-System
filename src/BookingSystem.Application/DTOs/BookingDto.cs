using BookingSystem.Domain.Enums;

namespace BookingSystem.Application.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; } // Class start time
    public DateTime CheckOutDate { get; set; } // Class end time
    public int NumberOfGuests { get; set; } = 1;
    public decimal TotalPrice { get; set; } = 0;
    public BookingStatus Status { get; set; }
    public int CreditsUsed { get; set; }
    public bool IsCheckedIn { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime CreatedAt { get; set; }
}
