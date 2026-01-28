using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room?> GetByRoomNumberAsync(string roomNumber);
    Task<IEnumerable<Room>> GetAllAsync();
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<Room> AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
