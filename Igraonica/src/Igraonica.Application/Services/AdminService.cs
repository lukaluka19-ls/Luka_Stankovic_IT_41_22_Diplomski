using Igraonica.Application.DTOs;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Repositories;

namespace Igraonica.Application.Services;

public class AdminService(
    IReservationRepository _reservationRepository,
    IPackageRepository _packageRepository,
    IAuditLogRepository _auditLogRepository) : IAdminService
{
    public async Task<StatisticsDto> GetStatisticsAsync(CancellationToken ct)
    {
        var filter = new ReservationFilter(null, null, null, null, 1, int.MaxValue);
        var all = await _reservationRepository.GetFilteredAsync(filter, ct);
        var packages = (await _packageRepository.GetAllAsync(ct)).ToDictionary(p => p.Id, p => p.Name);

        var reservations = all.Items;

        var popular = reservations
            .GroupBy(r => r.PackageId)
            .Select(g => new PackagePopularityDto(
                packages.TryGetValue(g.Key, out var name) ? name : "Nepoznat paket", g.Count()))
            .OrderByDescending(p => p.Count)
            .ToList();

        var revenue = reservations
            .Where(r => r.Status is ReservationStatus.Confirmed or ReservationStatus.Completed)
            .Sum(r => r.TotalPrice);

        return new StatisticsDto(
            TotalReservations: reservations.Count,
            PendingConfirmation: reservations.Count(r => r.Status == ReservationStatus.PendingConfirmation),
            Confirmed: reservations.Count(r => r.Status == ReservationStatus.Confirmed),
            Completed: reservations.Count(r => r.Status == ReservationStatus.Completed),
            CancelledByClient: reservations.Count(r => r.Status == ReservationStatus.CancelledByClient),
            CancelledByAdmin: reservations.Count(r => r.Status == ReservationStatus.CancelledByAdmin),
            AutoExpired: reservations.Count(r => r.Status == ReservationStatus.AutoExpired),
            TotalRevenue: revenue,
            PopularPackages: popular);
    }

    public async Task<PagedResultDto<AuditLogDto>> GetAuditLogsAsync(int page, int pageSize, CancellationToken ct)
    {
        var result = await _auditLogRepository.GetPagedAsync(page, pageSize, ct);
        var items = result.Items
            .Select(a => new AuditLogDto(a.Id, a.UserId, a.Action, a.EntityName, a.Details, a.Timestamp))
            .ToList();

        return new PagedResultDto<AuditLogDto>(items, result.TotalCount, result.Page, result.PageSize);
    }
}
