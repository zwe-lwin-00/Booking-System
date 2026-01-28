using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class UserPackageRepository : IUserPackageRepository
{
    private readonly ApplicationDbContext _context;

    public UserPackageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserPackage?> GetByIdAsync(Guid id)
    {
        return await _context.UserPackages
            .Include(up => up.Package)
            .ThenInclude(p => p.Country)
            .FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<IEnumerable<UserPackage>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserPackages
            .Include(up => up.Package)
            .ThenInclude(p => p.Country)
            .Where(up => up.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPackage>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.UserPackages
            .Include(up => up.Package)
            .ThenInclude(p => p.Country)
            .Where(up => up.UserId == userId && !up.IsExpired)
            .ToListAsync();
    }

    public async Task<UserPackage?> GetActiveByUserIdAndCountryIdAsync(Guid userId, Guid countryId)
    {
        return await _context.UserPackages
            .Include(up => up.Package)
            .ThenInclude(p => p.Country)
            .Where(up => up.UserId == userId && 
                        up.Package.CountryId == countryId && 
                        !up.IsExpired)
            .OrderByDescending(up => up.PurchaseDate)
            .FirstOrDefaultAsync();
    }

    public async Task<UserPackage> AddAsync(UserPackage userPackage)
    {
        userPackage.CreatedAt = DateTime.UtcNow;
        await _context.UserPackages.AddAsync(userPackage);
        await _context.SaveChangesAsync();
        return userPackage;
    }

    public async Task UpdateAsync(UserPackage userPackage)
    {
        userPackage.UpdatedAt = DateTime.UtcNow;
        _context.UserPackages.Update(userPackage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var userPackage = await _context.UserPackages.FindAsync(id);
        if (userPackage != null)
        {
            _context.UserPackages.Remove(userPackage);
            await _context.SaveChangesAsync();
        }
    }
}
