using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Register repositories
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
