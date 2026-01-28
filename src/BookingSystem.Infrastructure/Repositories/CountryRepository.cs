using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext _context;

    public CountryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Country?> GetByIdAsync(Guid id)
    {
        return await _context.Countries.FindAsync(id);
    }

    public async Task<Country?> GetByCodeAsync(string code)
    {
        return await _context.Countries
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<IEnumerable<Country>> GetAllAsync()
    {
        return await _context.Countries.ToListAsync();
    }

    public async Task<Country> AddAsync(Country country)
    {
        country.CreatedAt = DateTime.UtcNow;
        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();
        return country;
    }

    public async Task UpdateAsync(Country country)
    {
        country.UpdatedAt = DateTime.UtcNow;
        _context.Countries.Update(country);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country != null)
        {
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
        }
    }
}
