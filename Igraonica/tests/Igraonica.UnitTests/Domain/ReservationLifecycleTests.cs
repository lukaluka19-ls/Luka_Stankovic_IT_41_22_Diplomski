using FluentAssertions;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Exceptions;
using Igraonica.UnitTests.Helpers;
using Xunit;

namespace Igraonica.UnitTests.Domain;

public class ReservationLifecycleTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 5, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly EventDate = new(2026, 6, 20); // subota

    // T-05: otkazivanje sesnaest dana pre proslave - prihvaceno
    [Fact]
    public void Otkazivanje_sesnaest_dana_pre_prolazi()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate);
        var now = new DateTimeOffset(2026, 6, 4, 10, 0, 0, TimeSpan.Zero); // 16 dana pre

        var akcija = () => reservation.CancelByClient("Dete se razbolelo pa odlazemo proslavu.", now, 15);

        akcija.Should().NotThrow();
        reservation.Status.Should().Be(ReservationStatus.CancelledByClient);
    }

    // T-06: otkazivanje petnaest dana pre proslave - prihvaceno, granicni slucaj (PP-05)
    [Fact]
    public void Otkazivanje_petnaest_dana_pre_granicni_slucaj_prolazi()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate);
        var now = new DateTimeOffset(2026, 6, 5, 10, 0, 0, TimeSpan.Zero); // tacno 15 dana pre

        var akcija = () => reservation.CancelByClient("Dete se razbolelo pa odlazemo proslavu.", now, 15);

        akcija.Should().NotThrow();
    }

    // T-07: otkazivanje cetrnaest dana pre proslave - odbijeno
    [Fact]
    public void Otkazivanje_cetrnaest_dana_pre_se_odbija()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate);
        var now = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero); // 14 dana pre

        var akcija = () => reservation.CancelByClient("Dete se razbolelo pa odlazemo proslavu.", now, 15);

        akcija.Should().Throw<CancellationDeadlinePassedException>();
    }

    // T-08: otkazivanje bez navedenog razloga - odbijeno (PP-06)
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Otkazivanje_bez_razloga_se_odbija(string? reason)
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate);
        var now = new DateTimeOffset(2026, 6, 4, 10, 0, 0, TimeSpan.Zero);

        var akcija = () => reservation.CancelByClient(reason!, now, 15);

        akcija.Should().Throw<CancellationReasonRequiredException>();
    }

    // T-09: razlog kraci od deset znakova - odbijeno (PP-06)
    [Fact]
    public void Otkazivanje_sa_kratkim_razlogom_se_odbija()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate);
        var now = new DateTimeOffset(2026, 6, 4, 10, 0, 0, TimeSpan.Zero);

        var akcija = () => reservation.CancelByClient("Kratko", now, 15);

        akcija.Should().Throw<CancellationReasonRequiredException>();
    }

    // T-11: rezervacija stara sest dana bez potvrde prelazi u stanje isteklo (PP-04)
    [Fact]
    public void Rezervacija_stara_sest_dana_bez_potvrde_istice()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate, confirmationDeadlineDays: 5);
        var now = CreatedAt.AddDays(6);

        reservation.AutoExpire(now);

        reservation.Status.Should().Be(ReservationStatus.AutoExpired);
    }

    // T-12: rezervacija stara cetiri dana bez potvrde ostaje u stanju cekanja
    [Fact]
    public void Rezervacija_stara_cetiri_dana_ostaje_na_cekanju()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate, confirmationDeadlineDays: 5);
        var now = CreatedAt.AddDays(4);

        var akcija = () => reservation.AutoExpire(now);

        akcija.Should().Throw<ExpireTooEarlyException>();
        reservation.Status.Should().Be(ReservationStatus.PendingConfirmation);
    }

    // T-13: ponovno ponistavanje vec istekle rezervacije - bez dejstva (NFZ-08, idempotentnost)
    [Fact]
    public void Ponovno_ponistavanje_istekle_rezervacije_je_bez_dejstva()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate, confirmationDeadlineDays: 5);
        var now = CreatedAt.AddDays(6);
        reservation.AutoExpire(now);

        var akcija = () => reservation.AutoExpire(now.AddHours(1));

        akcija.Should().Throw<InvalidStateTransitionException>();
        reservation.Status.Should().Be(ReservationStatus.AutoExpired);
    }

    // T-15: potvrda rezervacije nakon isteka roka - odbijena
    [Fact]
    public void Potvrda_nakon_isteka_roka_se_odbija()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate, confirmationDeadlineDays: 5);
        var now = CreatedAt.AddDays(6);

        var akcija = () => reservation.ConfirmArrival(now);

        akcija.Should().Throw<ConfirmationDeadlinePassedException>();
    }

    [Fact]
    public void Potvrda_pre_isteka_roka_prolazi()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate, confirmationDeadlineDays: 5);
        var now = CreatedAt.AddDays(3);

        reservation.ConfirmArrival(now);

        reservation.Status.Should().Be(ReservationStatus.Confirmed);
        reservation.ConfirmedAt.Should().Be(now);
    }

    // T-16: otkazivanje vec otkazane rezervacije - odbijeno, nedozvoljen prelaz
    [Fact]
    public void Otkazivanje_vec_otkazane_rezervacije_se_odbija()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate);
        var now = new DateTimeOffset(2026, 6, 4, 10, 0, 0, TimeSpan.Zero);
        reservation.CancelByClient("Prvi razlog otkazivanja rezervacije.", now, 15);

        var akcija = () => reservation.CancelByClient("Drugi razlog otkazivanja rezervacije.", now, 15);

        akcija.Should().Throw<InvalidStateTransitionException>();
    }

    [Fact]
    public void Zavrsavanje_proslave_pre_datuma_se_odbija()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate, confirmationDeadlineDays: 5);
        reservation.ConfirmArrival(CreatedAt.AddDays(1));

        var akcija = () => reservation.Complete(CreatedAt.AddDays(1));

        akcija.Should().Throw<CompleteTooEarlyException>();
    }

    [Fact]
    public void Zavrsavanje_proslave_posle_datuma_prolazi()
    {
        var reservation = TestFactory.CreateReservation(CreatedAt, EventDate, confirmationDeadlineDays: 5);
        reservation.ConfirmArrival(CreatedAt.AddDays(1));

        var now = new DateTimeOffset(EventDate.ToDateTime(TimeOnly.MinValue).AddDays(1), TimeSpan.Zero);
        reservation.Complete(now);

        reservation.Status.Should().Be(ReservationStatus.Completed);
    }

    [Fact]
    public void Kreiranje_rezervacije_za_prosli_datum_se_odbija()
    {
        var proslostDatum = DateOnly.FromDateTime(CreatedAt.Date).AddDays(-1);

        var akcija = () => TestFactory.CreateReservation(CreatedAt, proslostDatum);

        akcija.Should().Throw<InvalidReservationDateException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    public void Kreiranje_rezervacije_sa_neispravnim_uzrastom_se_odbija(int childAge)
    {
        var package = TestFactory.CreatePackage();

        var akcija = () => Igraonica.Domain.Entities.Reservation.Create(
            Guid.NewGuid(), "Marko", childAge, EventDate, Guid.NewGuid(), 5, package, CreatedAt, 5);

        akcija.Should().Throw<ArgumentOutOfRangeException>();
    }
}
