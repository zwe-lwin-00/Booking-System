using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .Where(b => b.RoomId == roomId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime checkIn, DateTime checkOut)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .Where(b => (b.CheckInDate <= checkOut && b.CheckOutDate >= checkIn))
            .ToListAsync();
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        booking.CreatedAt = DateTime.UtcNow;
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Bookings.AnyAsync(b => b.Id == id);
    }
}
