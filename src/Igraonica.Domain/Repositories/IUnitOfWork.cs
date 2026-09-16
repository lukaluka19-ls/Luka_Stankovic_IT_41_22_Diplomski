using System.Data;

namespace Igraonica.Domain.Repositories;

public interface IUnitOfWork
{
    Task<IDbContextTransactionHandle> BeginTransactionAsync(IsolationLevel level, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IDbContextTransactionHandle : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}
