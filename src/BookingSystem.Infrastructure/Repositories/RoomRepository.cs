using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Rooms.FindAsync(id);
    }

    public async Task<Room?> GetByRoomNumberAsync(string roomNumber)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        var bookedRoomIds = await _context.Bookings
            .Where(b => b.Status == Domain.Enums.BookingStatus.Confirmed &&
                       (b.CheckInDate <= checkOut && b.CheckOutDate >= checkIn))
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync();

        return await _context.Rooms
            .Where(r => r.IsAvailable && !bookedRoomIds.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<Room> AddAsync(Room room)
    {
        room.CreatedAt = DateTime.UtcNow;
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task UpdateAsync(Room room)
    {
        room.UpdatedAt = DateTime.UtcNow;
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room != null)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == id);
    }
}
