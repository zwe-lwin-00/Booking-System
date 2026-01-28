using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Extensions;

public static class DataSeeder
{
    public static async Task SeedInitialDataAsync(ApplicationDbContext context)
    {
        // Check if data is already seeded (with error handling for missing tables)
        try
        {
            if (await context.Countries.AnyAsync())
                return; // Data already seeded
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 208) // Invalid object name
        {
            // Table doesn't exist yet, will be created - continue to seed
        }
        catch (Exception)
        {
            // Other database errors - log and continue
            // The EnsureCreated() call should have created the tables
        }

        // Seed Countries
        var singapore = new Country { Name = "Singapore", Code = "SG" };
        var myanmar = new Country { Name = "Myanmar", Code = "MM" };
        
        try
        {
            await context.Countries.AddRangeAsync(singapore, myanmar);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // If save fails, it might be because tables don't exist yet
            // This is okay - the next run will seed the data
            Console.WriteLine($"Warning: Could not seed initial data: {ex.Message}");
            return;
        }

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
