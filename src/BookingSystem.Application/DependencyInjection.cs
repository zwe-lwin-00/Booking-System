using BookingSystem.Application.Common.Settings;
using BookingSystem.Application.Services;
using BookingSystem.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure validation settings
        services.Configure<ValidationSettings>(
            configuration.GetSection(ValidationSettings.SectionName));

        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<CreateBookingDtoValidator>();

        // Register services
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
