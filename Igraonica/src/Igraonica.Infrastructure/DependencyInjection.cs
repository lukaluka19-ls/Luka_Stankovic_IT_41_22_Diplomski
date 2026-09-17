using Igraonica.Application.Abstractions;
using Igraonica.Domain.Repositories;
using Igraonica.Infrastructure.BackgroundJobs;
using Igraonica.Infrastructure.Persistence;
using Igraonica.Infrastructure.Persistence.Repositories;
using Igraonica.Infrastructure.Persistence.Seed;
using Igraonica.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Igraonica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationCancellationRepository, ReservationCancellationRepository>();
        services.AddScoped<IPackageRepository, PackageRepository>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<IReservationNumberGenerator, ReservationNumberGenerator>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddHostedService<ExpirationBackgroundService>();
        services.AddHostedService<OutboxProcessorService>();

        return services;
    }

    public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
}
