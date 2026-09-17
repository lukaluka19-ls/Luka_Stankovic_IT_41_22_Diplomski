using Igraonica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Igraonica.IntegrationTests.Infrastructure;

/// <summary>
/// Pokrece aplikaciju nad zasebnom test bazom "igraonica_test" na istoj
/// lokalnoj instanci PostgreSQL koja se koristi za razvoj. U okruzenju sa
/// dostupnim Docker-om ova fabrika bi umesto toga podigla efemernu instancu
/// pomocu biblioteke Testcontainers; ovde se koristi lokalna baza jer Docker
/// nije dostupan u razvojnom okruzenju u kome je projekat izradjen.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string ConnectionString =
        "Host=localhost;Port=5432;Database=igraonica_test;Username=igraonica_user;Password=igraonica_dev_2026";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = ConnectionString,
                ["Jwt:Key"] = "integration-test-signing-key-not-for-production-use",
                ["Email:Host"] = "localhost",
                ["Email:Port"] = "1025",
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE "ReservationCancellations", "Reservations", "RefreshTokens",
                "AuditLogs", "OutboxMessages", "Users" RESTART IDENTITY CASCADE;
            """);
    }
}
