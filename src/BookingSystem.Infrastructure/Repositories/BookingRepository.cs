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
            .Include(b => b.Class)
            .ThenInclude(c => c.Country)
            .Include(b => b.UserPackage)
            .ThenInclude(up => up.Package)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Class)
            .ThenInclude(c => c.Country)
            .Include(b => b.UserPackage)
            .ThenInclude(up => up.Package)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Class)
            .ThenInclude(c => c.Country)
            .Include(b => b.UserPackage)
            .ThenInclude(up => up.Package)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Class)
            .Include(b => b.UserPackage)
            .Where(b => b.ClassId == classId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAndTimeRangeAsync(Guid userId, DateTime startTime, DateTime endTime)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Class)
            .Include(b => b.UserPackage)
            .Where(b => b.UserId == userId &&
                       b.Status != Domain.Enums.BookingStatus.Cancelled &&
                       ((b.Class.StartTime <= endTime && b.Class.EndTime >= startTime)))
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
