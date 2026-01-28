using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IRoomService
{
    Task<RoomDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<RoomDto>> GetAllAsync();
    Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<RoomDto> CreateAsync(CreateRoomDto createRoomDto);
    Task<RoomDto> UpdateAsync(Guid id, CreateRoomDto updateRoomDto);
    Task DeleteAsync(Guid id);
}
