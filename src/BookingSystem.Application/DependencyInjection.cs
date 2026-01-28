using BookingSystem.Application.Common.Settings;
using BookingSystem.Application.Services;
using BookingSystem.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IUserPackageService, UserPackageService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
