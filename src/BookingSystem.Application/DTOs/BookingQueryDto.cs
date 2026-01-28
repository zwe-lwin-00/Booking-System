namespace BookingSystem.Application.DTOs;

public class BookingQueryDto
{
    public Guid? UserId { get; set; }
    public Guid? RoomId { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public Domain.Enums.BookingStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
