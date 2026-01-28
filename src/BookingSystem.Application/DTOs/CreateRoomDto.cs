namespace BookingSystem.Application.DTOs;

public class CreateRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string Description { get; set; } = string.Empty;
}
