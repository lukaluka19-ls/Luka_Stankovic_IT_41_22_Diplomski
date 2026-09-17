using Igraonica.Domain.Entities;

namespace Igraonica.Domain.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken ct);
    Task<PagedResult<AuditLog>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
}
