namespace Igraonica.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorCode { get; }

    protected DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public sealed class CapacityExceededException(string message)
    : DomainException("CAPACITY_EXCEEDED", message);

public sealed class EmailNotVerifiedException(string message)
    : DomainException("EMAIL_NOT_VERIFIED", message);

public sealed class EntityNotFoundException(string message)
    : DomainException("ENTITY_NOT_FOUND", message);

public sealed class CancellationDeadlinePassedException(string message)
    : DomainException("CANCELLATION_DEADLINE_PASSED", message);

public sealed class CancellationReasonRequiredException(string message)
    : DomainException("CANCELLATION_REASON_REQUIRED", message);

public sealed class InvalidStateTransitionException(string message)
    : DomainException("INVALID_STATE_TRANSITION", message);

public sealed class InvalidReservationDateException(string message)
    : DomainException("INVALID_RESERVATION_DATE", message);

public sealed class DuplicateEmailException(string message)
    : DomainException("DUPLICATE_EMAIL", message);

public sealed class InvalidCredentialsException(string message)
    : DomainException("INVALID_CREDENTIALS", message);

public sealed class InvalidTokenException(string message)
    : DomainException("INVALID_TOKEN", message);

public sealed class ConfirmationDeadlinePassedException(string message)
    : DomainException("CONFIRMATION_DEADLINE_PASSED", message);

public sealed class ExpireTooEarlyException(string message)
    : DomainException("EXPIRE_TOO_EARLY", message);

public sealed class CompleteTooEarlyException(string message)
    : DomainException("COMPLETE_TOO_EARLY", message);

public sealed class RateLimitExceededException(string message)
    : DomainException("RATE_LIMIT_EXCEEDED", message);

public sealed class ForbiddenException(string message)
    : DomainException("FORBIDDEN", message);

/// <summary>
/// Baza je odbila upis usled konflikta sa drugom istovremenom transakcijom
/// pod nivoom izolacije Serializable (SQLSTATE 40001). Aplikativni sloj ovo
/// obicno prevodi u konkretniji domenski izuzetak (npr. CapacityExceededException),
/// jer sam konflikt ne nosi poslovno znacenje.
/// </summary>
public sealed class ConcurrencyConflictException(string message)
    : DomainException("CONCURRENCY_CONFLICT", message);
