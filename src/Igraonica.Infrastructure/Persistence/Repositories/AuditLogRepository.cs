using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class AuditLogRepository(ApplicationDbContext _db) : IAuditLogRepository
{
    public async Task AddAsync(AuditLog log, CancellationToken ct) => await _db.AuditLogs.AddAsync(log, ct);

    public async Task<PagedResult<AuditLog>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _db.AuditLogs.OrderByDescending(a => a.Timestamp);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<AuditLog>(items, total, page, pageSize);
    }
}
