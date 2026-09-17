using FluentAssertions;
using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Exceptions;
using Igraonica.Infrastructure.Persistence;
using Igraonica.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Igraonica.IntegrationTests.Concurrency;

/// <summary>
/// T-18: deset istovremenih zahteva za isti radni dan - tacno jedan uspeva.
/// Ovaj test dokazuje ispravnost resenja za problem istovremenog pristupa
/// (transakcija sa nivoom izolacije Serializable + delimicni jedinstveni
/// indeks) i izvrsava se nad stvarnom instancom baze podataka.
/// </summary>
public class ConcurrentReservationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private Guid _packageId;
    private Guid _timeSlotId;
    private readonly List<Guid> _userIds = [];
    private static readonly DateOnly WorkDay = NextWeekday(DayOfWeek.Monday, 30);

    private static DateOnly NextWeekday(DayOfWeek dayOfWeek, int minDaysAhead)
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(minDaysAhead);
        while (date.DayOfWeek != dayOfWeek)
            date = date.AddDays(1);
        return date;
    }

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var package = Package.Create(PackageType.Basic, "Test paket", "Opis", 9000, 900);
        var timeSlot = TimeSlot.Create(new TimeOnly(10, 0), new TimeOnly(12, 0), "Test termin");
        db.Packages.Add(package);
        db.TimeSlots.Add(timeSlot);

        for (var i = 0; i < 10; i++)
        {
            var user = User.Register($"konkurentni{i}@example.com", hasher.Hash("Lozinka123!"),
                "Test", $"Korisnik{i}", "0640000000", $"token-{i}", clock.UtcNow);
            user.VerifyEmail($"token-{i}");
            db.Users.Add(user);
            _userIds.Add(user.Id);
        }

        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync(
            "CREATE SEQUENCE IF NOT EXISTS reservation_number_seq START WITH 1 INCREMENT BY 1;");

        _packageId = package.Id;
        _timeSlotId = timeSlot.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Samo_jedna_od_deset_istovremenih_rezervacija_uspeva()
    {
        var request = new CreateReservationRequest("Marko", 6, WorkDay, _timeSlotId, 5, _packageId);

        var tasks = _userIds.Select(async userId =>
        {
            using var scope = factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IReservationService>();
            try
            {
                await service.CreateAsync(request, userId, CancellationToken.None);
                return true;
            }
            catch (CapacityExceededException)
            {
                return false;
            }
        });

        var results = await Task.WhenAll(tasks);

        results.Count(success => success).Should().Be(1);
    }
}
