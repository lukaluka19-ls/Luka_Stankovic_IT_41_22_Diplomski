using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext _db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct) =>
        _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct) => await _db.RefreshTokens.AddAsync(token, ct);
}
