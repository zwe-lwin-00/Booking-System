using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly ApplicationDbContext _context;

    public PackageRepository(ApplicationDbContext context)
    {
        _context = context;
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
        return package;
    }

    public async Task UpdateAsync(Package package)
    {
        package.UpdatedAt = DateTime.UtcNow;
        _context.Packages.Update(package);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package != null)
        {
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
        }
    }
}
