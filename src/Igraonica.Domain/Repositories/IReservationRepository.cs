using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;

namespace Igraonica.Domain.Repositories;

public sealed record ReservationFilter(
    ReservationStatus? Status,
    DateOnly? FromDate,
    DateOnly? ToDate,
    Guid? UserId,
    int Page,
    int PageSize);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Reservation?> GetByReservationNumberAsync(string reservationNumber, CancellationToken ct);
    Task<int> CountActiveByDateAsync(DateOnly date, CancellationToken ct);
    Task<PagedResult<Reservation>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    Task<PagedResult<Reservation>> GetFilteredAsync(ReservationFilter filter, CancellationToken ct);
    Task<IReadOnlyList<Reservation>> GetDueForExpirationAsync(DateTimeOffset now, CancellationToken ct);
    Task<IReadOnlyList<Reservation>> GetDueForCompletionAsync(DateTimeOffset now, CancellationToken ct);
    Task<IReadOnlyDictionary<DateOnly, int>> GetActiveCountsInRangeAsync(
        DateOnly from, DateOnly to, CancellationToken ct);
    Task AddAsync(Reservation reservation, CancellationToken ct);
}
