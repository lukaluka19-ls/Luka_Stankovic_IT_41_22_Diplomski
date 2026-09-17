namespace Igraonica.Application.DTOs;

public sealed record ReservationDto(
    Guid Id,
    string ReservationNumber,
    Guid UserId,
    string ChildName,
    int ChildAge,
    DateOnly EventDate,
    Guid TimeSlotId,
    string TimeSlotName,
    int NumberOfChildren,
    Guid PackageId,
    string PackageName,
    string Status,
    decimal TotalPrice,
    DateTimeOffset CreatedAt,
    DateTimeOffset ConfirmationDeadline,
    DateTimeOffset? ConfirmedAt,
    DateTimeOffset? CancelledAt,
    int? DaysUntilDeadline);

public sealed record CreateReservationRequest(
    string ChildName, int ChildAge, DateOnly EventDate, Guid TimeSlotId,
    int NumberOfChildren, Guid PackageId);

public sealed record CancelReservationRequest(string Reason);

public sealed record AdminReservationFilterQuery(
    string? Status = null, DateOnly? FromDate = null, DateOnly? ToDate = null,
    Guid? UserId = null, int Page = 1, int PageSize = 20);
