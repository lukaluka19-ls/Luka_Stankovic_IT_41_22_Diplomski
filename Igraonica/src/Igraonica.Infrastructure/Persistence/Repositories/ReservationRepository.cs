using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class ReservationRepository(ApplicationDbContext _db) : IReservationRepository
{
    private static readonly ReservationStatus[] ActiveStatuses =
    [
        ReservationStatus.PendingConfirmation, ReservationStatus.Confirmed,
    ];

    public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Reservations.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<Reservation?> GetByReservationNumberAsync(string reservationNumber, CancellationToken ct) =>
        _db.Reservations.FirstOrDefaultAsync(r => r.ReservationNumber == reservationNumber, ct);

    public Task<int> CountActiveByDateAsync(DateOnly date, CancellationToken ct) =>
        _db.Reservations.CountAsync(
            r => r.EventDate == date && ActiveStatuses.Contains(r.Status), ct);

    public async Task<PagedResult<Reservation>> GetByUserAsync(
        Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Reservations.Where(r => r.UserId == userId).OrderByDescending(r => r.CreatedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Reservation>(items, total, page, pageSize);
    }

    public async Task<PagedResult<Reservation>> GetFilteredAsync(ReservationFilter filter, CancellationToken ct)
    {
        var query = _db.Reservations.AsQueryable();

        if (filter.Status is not null)
            query = query.Where(r => r.Status == filter.Status);

        if (filter.FromDate is not null)
            query = query.Where(r => r.EventDate >= filter.FromDate);

        if (filter.ToDate is not null)
            query = query.Where(r => r.EventDate <= filter.ToDate);

        if (filter.UserId is not null)
            query = query.Where(r => r.UserId == filter.UserId);

        query = query.OrderByDescending(r => r.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);
        return new PagedResult<Reservation>(items, total, filter.Page, filter.PageSize);
    }

    public async Task<IReadOnlyList<Reservation>> GetDueForExpirationAsync(DateTimeOffset now, CancellationToken ct) =>
        await _db.Reservations
            .Where(r => r.Status == ReservationStatus.PendingConfirmation && r.ConfirmationDeadline <= now)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Reservation>> GetDueForCompletionAsync(DateTimeOffset now, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(now.Date);
        return await _db.Reservations
            .Where(r => r.Status == ReservationStatus.Confirmed && r.EventDate < today)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyDictionary<DateOnly, int>> GetActiveCountsInRangeAsync(
        DateOnly from, DateOnly to, CancellationToken ct)
    {
        var grouped = await _db.Reservations
            .Where(r => r.EventDate >= from && r.EventDate <= to && ActiveStatuses.Contains(r.Status))
            .GroupBy(r => r.EventDate)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return grouped.ToDictionary(g => g.Date, g => g.Count);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken ct) =>
        await _db.Reservations.AddAsync(reservation, ct);
}
