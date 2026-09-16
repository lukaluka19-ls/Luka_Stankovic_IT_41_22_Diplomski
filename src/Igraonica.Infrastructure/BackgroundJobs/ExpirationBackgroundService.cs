using Igraonica.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Igraonica.Infrastructure.BackgroundJobs;

/// <summary>
/// Pozadinski posao koji na svakih sat vremena ponistava rezervacije koje
/// nisu fizicki potvrdjene u roku (PP-04) i zavrsava potvrdjene rezervacije
/// cija je proslava prosla. Idempotentan je: ponovno pokretanje nad vec
/// obradjenom rezervacijom nema dejstva, jer domenska metoda odbija prelaz
/// iz zavrsnog stanja (NFZ-08).
/// </summary>
public class ExpirationBackgroundService(
    IServiceScopeFactory _scopeFactory,
    ILogger<ExpirationBackgroundService> _logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Servisi koji rade sa bazom ne mogu se koristiti izvan
                // opsega zahteva, pa posao kreira sopstveni opseg.
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IReservationService>();

                var expiredCount = await service.ExpireOverdueReservationsAsync(stoppingToken);
                var completedCount = await service.CompleteOverdueReservationsAsync(stoppingToken);

                if (expiredCount > 0 || completedCount > 0)
                    _logger.LogInformation(
                        "Ponisteno {ExpiredCount} isteklih rezervacija, zavrseno {CompletedCount} odrzanih proslava.",
                        expiredCount, completedCount);
            }
            catch (Exception ex)
            {
                // Greska ne sme da zaustavi posao - pokusace se ponovo u
                // narednom ciklusu.
                _logger.LogError(ex, "Greska pri obradi isteklih/zavrsenih rezervacija.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Ocekivano pri gasenju aplikacije.
            }
        }
    }
}
