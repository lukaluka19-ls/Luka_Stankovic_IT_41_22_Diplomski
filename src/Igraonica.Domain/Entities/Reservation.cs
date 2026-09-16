using Igraonica.Domain.Common;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Exceptions;

namespace Igraonica.Domain.Entities;

public class Reservation : BaseEntity
{
    private static readonly Dictionary<ReservationStatus, ReservationStatus[]> AllowedTransitions = new()
    {
        [ReservationStatus.PendingConfirmation] =
        [
            ReservationStatus.Confirmed, ReservationStatus.CancelledByClient,
            ReservationStatus.CancelledByAdmin, ReservationStatus.AutoExpired,
        ],
        [ReservationStatus.Confirmed] =
        [
            ReservationStatus.CancelledByClient, ReservationStatus.CancelledByAdmin,
            ReservationStatus.Completed,
        ],
        [ReservationStatus.CancelledByClient] = [],
        [ReservationStatus.CancelledByAdmin] = [],
        [ReservationStatus.AutoExpired] = [],
        [ReservationStatus.Completed] = [],
    };

    public string ReservationNumber { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public string ChildName { get; private set; } = null!;
    public int ChildAge { get; private set; }
    public DateOnly EventDate { get; private set; }
    public Guid TimeSlotId { get; private set; }
    public int NumberOfChildren { get; private set; }
    public Guid PackageId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ConfirmationDeadline { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }

    private Reservation() { } // za EF Core

    public static Reservation Create(Guid userId, string childName, int childAge,
        DateOnly eventDate, Guid timeSlotId, int numberOfChildren, Package package,
        DateTimeOffset now, int confirmationDeadlineDays)
    {
        if (eventDate <= DateOnly.FromDateTime(now.Date))
            throw new InvalidReservationDateException("Datum proslave mora biti u buducnosti.");

        if (childAge is < 1 or > 14)
            throw new ArgumentOutOfRangeException(nameof(childAge), "Uzrast deteta mora biti izmedju 1 i 14 godina.");

        if (numberOfChildren is < 1 or > 40)
            throw new ArgumentOutOfRangeException(nameof(numberOfChildren), "Broj dece mora biti izmedju 1 i 40.");

        return new Reservation
        {
            UserId = userId,
            ChildName = childName.Trim(),
            ChildAge = childAge,
            EventDate = eventDate,
            TimeSlotId = timeSlotId,
            NumberOfChildren = numberOfChildren,
            PackageId = package.Id,
            TotalPrice = package.CalculatePrice(numberOfChildren),
            Status = ReservationStatus.PendingConfirmation,
            CreatedAt = now,
            ConfirmationDeadline = now.AddDays(confirmationDeadlineDays),
        };
    }

    public void AssignNumber(string reservationNumber)
    {
        ReservationNumber = reservationNumber;
    }

    /// <summary>
    /// Otkazivanje od strane klijenta - najkasnije "noticeDays" dana pre proslave,
    /// uz obavezan razlog dužine najmanje deset znakova (PP-05, PP-06).
    /// </summary>
    public void CancelByClient(string reason, DateTimeOffset now, int noticeDays)
    {
        EnsureTransitionAllowed(ReservationStatus.CancelledByClient);

        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < 10)
            throw new CancellationReasonRequiredException(
                "Razlog otkazivanja je obavezan i mora imati najmanje deset znakova.");

        var daysUntilEvent = EventDate.DayNumber - DateOnly.FromDateTime(now.Date).DayNumber;

        if (daysUntilEvent < noticeDays)
            throw new CancellationDeadlinePassedException(
                $"Rezervacija se moze otkazati najkasnije {noticeDays} dana pre proslave. " +
                $"Do proslave je preostalo {daysUntilEvent} dana.");

        Status = ReservationStatus.CancelledByClient;
        CancelledAt = now;
    }

    /// <summary>
    /// Otkazivanje ili brisanje od strane administratora - bez ogranicenja u pogledu roka,
    /// jer administrator postupa u ime igraonice.
    /// </summary>
    public void CancelByAdmin(DateTimeOffset now)
    {
        EnsureTransitionAllowed(ReservationStatus.CancelledByAdmin);

        Status = ReservationStatus.CancelledByAdmin;
        CancelledAt = now;
    }

    /// <summary>
    /// Administrator potvrdjuje fizicki dolazak roditelja u roku od pet dana (PP-03).
    /// </summary>
    public void ConfirmArrival(DateTimeOffset now)
    {
        EnsureTransitionAllowed(ReservationStatus.Confirmed);

        if (now > ConfirmationDeadline)
            throw new ConfirmationDeadlinePassedException("Rok za fizicku potvrdu je istekao.");

        Status = ReservationStatus.Confirmed;
        ConfirmedAt = now;
    }

    /// <summary>
    /// Automatsko ponistavanje - poziva ga iskljucivo pozadinski posao (PP-04).
    /// Idempotentno: ako rok jos nije istekao ili je rezervacija vec u zavrsnom
    /// stanju, poziv nema dejstva odn. baca izuzetak koji pozadinski posao ignorise.
    /// </summary>
    public void AutoExpire(DateTimeOffset now)
    {
        EnsureTransitionAllowed(ReservationStatus.AutoExpired);

        if (now < ConfirmationDeadline)
            throw new ExpireTooEarlyException("Rok za potvrdu jos nije istekao.");

        Status = ReservationStatus.AutoExpired;
    }

    /// <summary>
    /// Proslava je odrzana - datum je prosao za potvrdjenu rezervaciju.
    /// Poziva ga pozadinski posao.
    /// </summary>
    public void Complete(DateTimeOffset now)
    {
        EnsureTransitionAllowed(ReservationStatus.Completed);

        if (DateOnly.FromDateTime(now.Date) <= EventDate)
            throw new CompleteTooEarlyException("Datum proslave jos nije prosao.");

        Status = ReservationStatus.Completed;
    }

    public bool CanBeAutoExpired(DateTimeOffset now) =>
        Status == ReservationStatus.PendingConfirmation && now >= ConfirmationDeadline;

    public bool CanBeCompleted(DateTimeOffset now) =>
        Status == ReservationStatus.Confirmed && DateOnly.FromDateTime(now.Date) > EventDate;

    private void EnsureTransitionAllowed(ReservationStatus target)
    {
        if (!AllowedTransitions[Status].Contains(target))
            throw new InvalidStateTransitionException(
                $"Prelaz iz stanja {Status} u stanje {target} nije dozvoljen.");
    }
}
