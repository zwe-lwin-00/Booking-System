using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Infrastructure.Repositories;

public class WaitlistRepository : IWaitlistRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WaitlistRepository> _logger;

    public WaitlistRepository(ApplicationDbContext context, ILogger<WaitlistRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Waitlist?> GetByIdAsync(Guid id)
    {
        return await _context.Waitlists
            .Include(w => w.User)
            .Include(w => w.Class)
            .Include(w => w.UserPackage)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<Waitlist>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Waitlists
            .Include(w => w.User)
            .Include(w => w.Class)
            .Include(w => w.UserPackage)
            .Where(w => w.ClassId == classId)
            .OrderBy(w => w.Position)
            .ToListAsync();
    }

    public async Task<IEnumerable<Waitlist>> GetPendingByClassIdAsync(Guid classId)
    {
        return await _context.Waitlists
            .Include(w => w.User)
            .Include(w => w.Class)
            .Include(w => w.UserPackage)
            .Where(w => w.ClassId == classId && !w.IsPromoted)
            .OrderBy(w => w.Position)
            .ToListAsync();
    }

    public async Task<Waitlist?> GetNextInWaitlistAsync(Guid classId)
    {
        return await _context.Waitlists
            .Include(w => w.User)
            .Include(w => w.Class)
            .Include(w => w.UserPackage)
            .Where(w => w.ClassId == classId && !w.IsPromoted)
            .OrderBy(w => w.Position)
            .FirstOrDefaultAsync();
    }

    public async Task<Waitlist> AddAsync(Waitlist waitlist)
    {
        waitlist.CreatedAt = DateTime.UtcNow;
        await _context.Waitlists.AddAsync(waitlist);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Waitlist entry added. WaitlistId: {WaitlistId}, UserId: {UserId}, ClassId: {ClassId}, Position: {Position}", waitlist.Id, waitlist.UserId, waitlist.ClassId, waitlist.Position);
        return waitlist;
    }

    public async Task UpdateAsync(Waitlist waitlist)
    {
        waitlist.UpdatedAt = DateTime.UtcNow;
        _context.Waitlists.Update(waitlist);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Waitlist entry updated. WaitlistId: {WaitlistId}", waitlist.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var waitlist = await _context.Waitlists.FindAsync(id);
        if (waitlist != null)
        {
            _context.Waitlists.Remove(waitlist);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Waitlist entry deleted. WaitlistId: {WaitlistId}", id);
        }
        else
        {
            _logger.LogWarning("Delete waitlist: waitlist {WaitlistId} not found", id);
        }
    }

    public async Task<int> GetNextPositionAsync(Guid classId)
    {
        var maxPosition = await _context.Waitlists
            .Where(w => w.ClassId == classId)
            .MaxAsync(w => (int?)w.Position) ?? 0;
        
        return maxPosition + 1;
    }
}
