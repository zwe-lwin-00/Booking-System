namespace BookingSystem.Application.DTOs;

public class RoomQueryDto
{
    public string? RoomType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinCapacity { get; set; }
    public bool? IsAvailable { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
