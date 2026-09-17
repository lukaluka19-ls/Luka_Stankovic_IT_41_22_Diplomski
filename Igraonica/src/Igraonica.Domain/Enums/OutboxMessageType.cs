namespace Igraonica.Domain.Enums;

public enum OutboxMessageType
{
    EmailVerification,
    ReservationCreated,
    ReservationConfirmed,
    ReservationCancelled,
    ReservationAutoExpired,
    PasswordReset,
}
