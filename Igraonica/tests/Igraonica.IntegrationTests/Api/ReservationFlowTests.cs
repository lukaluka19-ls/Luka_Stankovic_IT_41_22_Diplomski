using FluentAssertions;
using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Igraonica.Infrastructure.Persistence;
using Igraonica.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Igraonica.IntegrationTests.Api;

/// <summary>
/// T-17: otkazana rezervacija ne umanjuje kapacitet - termin ponovo slobodan.
/// Proverava saradnju domenskog sloja, EF Core repozitorijuma i delimicnog
/// jedinstvenog indeksa nad stvarnom bazom podataka.
/// </summary>
public class ReservationFlowTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private Guid _packageId;
    private Guid _timeSlotId;
    private Guid _userId;
    private static readonly DateOnly WorkDay = NextWeekday(DayOfWeek.Tuesday, 30);

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
        var user = User.Register("tok17@example.com", hasher.Hash("Lozinka123!"),
            "Test", "Korisnik", "0640000000", "token-17", clock.UtcNow);
        user.VerifyEmail("token-17");

        db.AddRange(package, timeSlot, user);
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync(
            "CREATE SEQUENCE IF NOT EXISTS reservation_number_seq START WITH 1 INCREMENT BY 1;");

        _packageId = package.Id;
        _timeSlotId = timeSlot.Id;
        _userId = user.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Otkazana_rezervacija_oslobadja_termin_za_novu_rezervaciju()
    {
        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IReservationService>();

        var createRequest = new CreateReservationRequest("Marko", 6, WorkDay, _timeSlotId, 5, _packageId);
        var created = await service.CreateAsync(createRequest, _userId, CancellationToken.None);

        await service.CancelByClientAsync(created.Id, _userId,
            "Dete se razbolelo pa odlazemo proslavu.", CancellationToken.None);

        // Isti termin ponovo slobodan - nova rezervacija za isti datum treba da prodje.
        var second = await service.CreateAsync(createRequest, _userId, CancellationToken.None);

        second.Status.Should().Be(nameof(ReservationStatus.PendingConfirmation));
        second.EventDate.Should().Be(WorkDay);
    }
}
