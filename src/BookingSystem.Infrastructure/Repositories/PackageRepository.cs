using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Infrastructure.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PackageRepository> _logger;

    public PackageRepository(ApplicationDbContext context, ILogger<PackageRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Package?> GetByIdAsync(Guid id)
    {
        return await _context.Packages
            .Include(p => p.Country)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Package>> GetAllAsync()
    {
        return await _context.Packages
            .Include(p => p.Country)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetByCountryIdAsync(Guid countryId)
    {
        return await _context.Packages
            .Include(p => p.Country)
            .Where(p => p.CountryId == countryId)
            .ToListAsync();
    }

    public async Task<Package> AddAsync(Package package)
    {
        package.CreatedAt = DateTime.UtcNow;
        await _context.Packages.AddAsync(package);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Package added. PackageId: {PackageId}, Name: {Name}", package.Id, package.Name);
        return package;
    }

    public async Task UpdateAsync(Package package)
    {
        package.UpdatedAt = DateTime.UtcNow;
        _context.Packages.Update(package);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Package updated. PackageId: {PackageId}", package.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package != null)
        {
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Package deleted. PackageId: {PackageId}", id);
        }
        else
        {
            _logger.LogWarning("Delete package: package {PackageId} not found", id);
        }
    }
}
