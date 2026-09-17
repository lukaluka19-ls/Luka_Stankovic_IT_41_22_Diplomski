using Igraonica.Domain.Entities;

namespace Igraonica.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
    Task AddAsync(RefreshToken token, CancellationToken ct);
}
