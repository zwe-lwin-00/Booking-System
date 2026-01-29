using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Infrastructure.Repositories;

public class UserPackageRepository : IUserPackageRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserPackageRepository> _logger;

    public UserPackageRepository(ApplicationDbContext context, ILogger<UserPackageRepository> logger)
    {
        _context = context;
        _logger = logger;
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
        _logger.LogInformation("UserPackage added. UserPackageId: {UserPackageId}, UserId: {UserId}, PackageId: {PackageId}", userPackage.Id, userPackage.UserId, userPackage.PackageId);
        return userPackage;
    }

    public async Task UpdateAsync(UserPackage userPackage)
    {
        userPackage.UpdatedAt = DateTime.UtcNow;
        _context.UserPackages.Update(userPackage);
        await _context.SaveChangesAsync();
        _logger.LogInformation("UserPackage updated. UserPackageId: {UserPackageId}", userPackage.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userPackage = await _context.UserPackages.FindAsync(id);
        if (userPackage != null)
        {
            _context.UserPackages.Remove(userPackage);
            await _context.SaveChangesAsync();
            _logger.LogInformation("UserPackage deleted. UserPackageId: {UserPackageId}", id);
        }
        else
        {
            _logger.LogWarning("Delete UserPackage: UserPackageId {UserPackageId} not found", id);
        }
    }
}
