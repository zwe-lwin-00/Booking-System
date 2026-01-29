using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Infrastructure.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClassRepository> _logger;

    public ClassRepository(ApplicationDbContext context, ILogger<ClassRepository> logger)
    {
        _context = context;
        _logger = logger;
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
        _logger.LogInformation("Class added. ClassId: {ClassId}, Name: {Name}", classEntity.Id, classEntity.Name);
        return classEntity;
    }

    public async Task UpdateAsync(Class classEntity)
    {
        classEntity.UpdatedAt = DateTime.UtcNow;
        _context.Classes.Update(classEntity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Class updated. ClassId: {ClassId}", classEntity.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var classEntity = await _context.Classes.FindAsync(id);
        if (classEntity != null)
        {
            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Class deleted. ClassId: {ClassId}", id);
        }
        else
        {
            _logger.LogWarning("Delete class: class {ClassId} not found", id);
        }
    }

    public async Task IncrementBookingCountAsync(Guid classId)
    {
        var classEntity = await _context.Classes.FindAsync(classId);
        if (classEntity != null)
        {
            classEntity.CurrentBookings++;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Class booking count incremented. ClassId: {ClassId}, CurrentBookings: {Count}", classId, classEntity.CurrentBookings);
        }
        else
        {
            _logger.LogWarning("Increment booking count: class {ClassId} not found", classId);
        }
    }

    public async Task DecrementBookingCountAsync(Guid classId)
    {
        var classEntity = await _context.Classes.FindAsync(classId);
        if (classEntity != null && classEntity.CurrentBookings > 0)
        {
            classEntity.CurrentBookings--;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Class booking count decremented. ClassId: {ClassId}, CurrentBookings: {Count}", classId, classEntity.CurrentBookings);
        }
        else if (classEntity == null)
        {
            _logger.LogWarning("Decrement booking count: class {ClassId} not found", classId);
        }
    }
}
