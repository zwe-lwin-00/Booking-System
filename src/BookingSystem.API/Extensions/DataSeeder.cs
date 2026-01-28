using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace BookingSystem.API.Extensions;

public static class DataSeeder
{
    public static async Task SeedInitialDataAsync(ApplicationDbContext context)
    {
        // Check if data is already seeded (with error handling for missing tables)
        try
        {
            // Try to check if data exists - this will fail if table doesn't exist
            if (await context.Countries.AnyAsync())
            {
                Console.WriteLine("Data already seeded. Skipping.");
                return; // Data already seeded
            }
        }
        catch (SqlException ex) when (ex.Number == 208) // Invalid object name
        {
            // Table doesn't exist yet - wait for EnsureCreated to finish
            Console.WriteLine("Warning: Countries table not found. Waiting for table creation...");
            await Task.Delay(2000);
            
            // Retry check after waiting
            try
            {
                if (await context.Countries.AnyAsync())
                {
                    Console.WriteLine("Data already seeded. Skipping.");
                    return;
                }
            }
            catch (SqlException retryEx) when (retryEx.Number == 208)
            {
                Console.WriteLine("Warning: Countries table still does not exist. Skipping seed.");
                return;
            }
            catch (Exception retryEx)
            {
                Console.WriteLine($"Warning: Error checking table: {retryEx.Message}. Skipping seed.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Error checking existing data: {ex.Message}. Attempting to seed anyway...");
        }

        // Seed Countries
        try
        {
            var singapore = new Country { Name = "Singapore", Code = "SG" };
            var myanmar = new Country { Name = "Myanmar", Code = "MM" };
            
            await context.Countries.AddRangeAsync(singapore, myanmar);
            await context.SaveChangesAsync();
            Console.WriteLine("Successfully seeded Countries: Singapore (SG), Myanmar (MM)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Countries: {ex.Message}");
            // Don't throw - allow app to continue
        }

        // Seed Packages (only if Countries were seeded successfully)
        try
        {
            // Get the countries we just created
            var singapore = await context.Countries.FirstOrDefaultAsync(c => c.Code == "SG");
            var myanmar = await context.Countries.FirstOrDefaultAsync(c => c.Code == "MM");

            if (singapore == null || myanmar == null)
            {
                Console.WriteLine("Warning: Countries not found. Cannot seed packages.");
                return;
            }

            // Check if packages already exist
            if (await context.Packages.AnyAsync())
            {
                Console.WriteLine("Packages already seeded. Skipping.");
                return;
            }

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
            Console.WriteLine("Successfully seeded Packages: Basic Package SG, Premium Package SG, Basic Package MM");
        }
        catch (SqlException ex) when (ex.Number == 208) // Invalid object name
        {
            Console.WriteLine("Warning: Packages table does not exist. Skipping package seed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Packages: {ex.Message}");
            // Don't throw - allow app to continue
        }
    }
}
