using Igraonica.Application.DTOs;

namespace Igraonica.Application.Services;

public interface IReservationService
{
    Task<ReservationDto> CreateAsync(CreateReservationRequest request, Guid userId, CancellationToken ct);
    Task CancelByClientAsync(Guid id, Guid userId, string reason, CancellationToken ct);
    Task CancelByAdminAsync(Guid id, string? reason, CancellationToken ct);
    Task<ReservationDto> ConfirmArrivalAsync(Guid id, CancellationToken ct);
    Task<ReservationDto> GetByIdAsync(Guid id, Guid requestingUserId, bool isAdmin, CancellationToken ct);
    Task<PagedResultDto<ReservationDto>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    Task<PagedResultDto<ReservationDto>> GetFilteredAsync(AdminReservationFilterQuery query, CancellationToken ct);
    Task<int> ExpireOverdueReservationsAsync(CancellationToken ct);
    Task<int> CompleteOverdueReservationsAsync(CancellationToken ct);
}
