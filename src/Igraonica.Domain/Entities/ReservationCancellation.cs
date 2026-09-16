using Igraonica.Domain.Common;

namespace Igraonica.Domain.Entities;

public class ReservationCancellation : BaseEntity
{
    public Guid ReservationId { get; private set; }
    public string Reason { get; private set; } = null!;
    public DateTimeOffset CancelledAt { get; private set; }
    public Guid CancelledByUserId { get; private set; }

    private ReservationCancellation() { }

    public static ReservationCancellation Create(Guid reservationId, string reason,
        DateTimeOffset now, Guid cancelledByUserId)
    {
        return new ReservationCancellation
        {
            ReservationId = reservationId,
            Reason = reason.Trim(),
            CancelledAt = now,
            CancelledByUserId = cancelledByUserId,
        };
    }
}
