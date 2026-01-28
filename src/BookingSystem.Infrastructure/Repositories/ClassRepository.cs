using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDbContext _context;

    public ClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _context.Classes
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Class>> GetAllAsync()
    {
        return await _context.Classes
            .Include(c => c.Country)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetByCountryIdAsync(Guid countryId)
    {
        return await _context.Classes
            .Include(c => c.Country)
            .Where(c => c.CountryId == countryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetUpcomingAsync(DateTime fromDate)
    {
        return await _context.Classes
            .Include(c => c.Country)
            .Where(c => c.StartTime >= fromDate)
            .OrderBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<Class> AddAsync(Class classEntity)
    {
        classEntity.CreatedAt = DateTime.UtcNow;
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();
        return classEntity;
    }

    public async Task UpdateAsync(Class classEntity)
    {
        classEntity.UpdatedAt = DateTime.UtcNow;
        _context.Classes.Update(classEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var classEntity = await _context.Classes.FindAsync(id);
        if (classEntity != null)
        {
            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task IncrementBookingCountAsync(Guid classId)
    {
        var classEntity = await _context.Classes.FindAsync(classId);
        if (classEntity != null)
        {
            classEntity.CurrentBookings++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DecrementBookingCountAsync(Guid classId)
    {
        var classEntity = await _context.Classes.FindAsync(classId);
        if (classEntity != null && classEntity.CurrentBookings > 0)
        {
            classEntity.CurrentBookings--;
            await _context.SaveChangesAsync();
        }
    }
}
