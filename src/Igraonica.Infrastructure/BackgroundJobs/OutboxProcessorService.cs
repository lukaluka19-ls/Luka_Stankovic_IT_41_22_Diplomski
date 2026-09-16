using Igraonica.Application.Abstractions;
using Igraonica.Domain.Repositories;
using Igraonica.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Igraonica.Infrastructure.BackgroundJobs;

/// <summary>
/// Salje poruke elektronske poste upisane u tabelu OutboxMessages, na svakih
/// trideset sekundi. Poruka se upisuje u istoj transakciji u kojoj se snima
/// rezervacija (ili druga poslovna promena), pa neuspeh slanja poste ne moze
/// ponistiti vec sacuvan poslovni podatak (NFZ-07).
/// </summary>
public class OutboxProcessorService(
    IServiceScopeFactory _scopeFactory,
    ILogger<OutboxProcessorService> _logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);
    private const int BatchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

                var messages = await outbox.GetUnsentAsync(BatchSize, stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        await emailService.SendAsync(message.Recipient, message.Subject, message.Body, stoppingToken);
                        message.MarkSent(clock.UtcNow);
                    }
                    catch (Exception ex)
                    {
                        message.RegisterFailure(ex.Message);
                        _logger.LogWarning(ex, "Neuspesno slanje poruke {MessageId} na {Recipient}.",
                            message.Id, message.Recipient);
                    }
                }

                if (messages.Count > 0)
                    await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greska pri obradi poruka na cekanju.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
