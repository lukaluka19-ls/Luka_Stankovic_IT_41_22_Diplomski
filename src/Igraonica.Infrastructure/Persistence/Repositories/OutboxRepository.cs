using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class OutboxRepository(ApplicationDbContext _db) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message, CancellationToken ct) =>
        await _db.OutboxMessages.AddAsync(message, ct);

    public async Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync(int batchSize, CancellationToken ct) =>
        await _db.OutboxMessages
            .Where(m => m.SentAt == null && m.RetryCount < 5)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);
}
