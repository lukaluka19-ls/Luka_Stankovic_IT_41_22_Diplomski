using System.Data;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Igraonica.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext _db) : IUnitOfWork
{
    public async Task<IDbContextTransactionHandle> BeginTransactionAsync(IsolationLevel level, CancellationToken ct)
    {
        var transaction = await _db.Database.BeginTransactionAsync(level, ct);
        return new TransactionHandle(transaction);
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    private sealed class TransactionHandle(IDbContextTransaction _transaction) : IDbContextTransactionHandle
    {
        public Task CommitAsync(CancellationToken ct) => _transaction.CommitAsync(ct);
        public Task RollbackAsync(CancellationToken ct) => _transaction.RollbackAsync(ct);
        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
