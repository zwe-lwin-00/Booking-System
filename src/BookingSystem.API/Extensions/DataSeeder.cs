using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Extensions;

public static class DataSeeder
{
    public static async Task SeedInitialDataAsync(ApplicationDbContext context)
    {
        if (await context.Countries.AnyAsync())
            return; // Data already seeded

        // Seed Countries
        var singapore = new Country { Name = "Singapore", Code = "SG" };
        var myanmar = new Country { Name = "Myanmar", Code = "MM" };
        
        await context.Countries.AddRangeAsync(singapore, myanmar);
        await context.SaveChangesAsync();

        // Seed Packages
        var packages = new List<Package>
        {
            new Package
            {
                Name = "Basic Package SG",
                CountryId = singapore.Id,
                Credits = 5,
                Price = 50.00m,
                ExpiryDate = DateTime.UtcNow.AddMonths(3),
                IsActive = true
            },
            new Package
            {
                Name = "Premium Package SG",
                CountryId = singapore.Id,
                Credits = 10,
                Price = 90.00m,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsActive = true
            },
            new Package
            {
                Name = "Basic Package MM",
                CountryId = myanmar.Id,
                Credits = 5,
                Price = 30.00m,
                ExpiryDate = DateTime.UtcNow.AddMonths(3),
                IsActive = true
            }
        };

        await context.Packages.AddRangeAsync(packages);
        await context.SaveChangesAsync();
    }
}
