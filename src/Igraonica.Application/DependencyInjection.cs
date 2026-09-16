using FluentValidation;
using Igraonica.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Igraonica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<ITimeSlotService, TimeSlotService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}
