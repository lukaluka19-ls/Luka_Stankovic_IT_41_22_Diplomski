using System.Data;
using Igraonica.Domain.Exceptions;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace Igraonica.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext _db) : IUnitOfWork
{
    public async Task<IDbContextTransactionHandle> BeginTransactionAsync(IsolationLevel level, CancellationToken ct)
    {
        var transaction = await _db.Database.BeginTransactionAsync(level, ct);
        return new TransactionHandle(transaction);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (IsSerializationFailure(ex))
        {
            // Pod nivoom izolacije Serializable, PostgreSQL odbija upis ako bi
            // narusio serijalizabilnost sa drugom istovremenom transakcijom
            // (SQLSTATE 40001). EF Core ovu prolaznu gresku moze da umota u
            // DbUpdateException ili, kada transakcijom rukuje pozivalac (a ne
            // strategija izvrsavanja), u InvalidOperationException - zato se
            // ceo lanac izuzetaka pretrazuje. Ovo je ocekivan ishod "gubitnika"
            // u trci za isti termin, pa se prevodi u domenski izuzetak.
            throw new ConcurrencyConflictException(
                "Termin je u medjuvremenu zauzet od strane druge rezervacije.");
        }
    }

    private static bool IsSerializationFailure(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException)
        {
            if (current is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure })
                return true;
        }

        return false;
    }

    private sealed class TransactionHandle(IDbContextTransaction _transaction) : IDbContextTransactionHandle
    {
        public async Task CommitAsync(CancellationToken ct)
        {
            try
            {
                await _transaction.CommitAsync(ct);
            }
            catch (Exception ex) when (IsSerializationFailure(ex))
            {
                throw new ConcurrencyConflictException(
                    "Termin je u medjuvremenu zauzet od strane druge rezervacije.");
            }
        }

        public Task RollbackAsync(CancellationToken ct) => _transaction.RollbackAsync(ct);
        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
