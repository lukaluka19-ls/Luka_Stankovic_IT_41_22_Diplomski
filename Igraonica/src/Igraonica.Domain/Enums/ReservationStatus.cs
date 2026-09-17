namespace Igraonica.Domain.Enums;

public enum ReservationStatus : short
{
    PendingConfirmation = 0,
    Confirmed = 1,
    CancelledByClient = 2,
    CancelledByAdmin = 3,
    AutoExpired = 4,
    Completed = 5,
}
