using BookingSystem.Application.Services;
using BookingSystem.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<CreateBookingDtoValidator>();

        // Register services
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
