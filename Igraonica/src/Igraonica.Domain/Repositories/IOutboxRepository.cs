using Igraonica.Domain.Entities;

namespace Igraonica.Domain.Repositories;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken ct);
    Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync(int batchSize, CancellationToken ct);
}
