using Igraonica.Application.DTOs;

namespace Igraonica.Application.Services;

public interface IAdminService
{
    Task<StatisticsDto> GetStatisticsAsync(CancellationToken ct);
    Task<PagedResultDto<AuditLogDto>> GetAuditLogsAsync(int page, int pageSize, CancellationToken ct);
}
