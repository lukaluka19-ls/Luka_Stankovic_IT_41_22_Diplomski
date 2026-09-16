using Igraonica.Domain.Entities;

namespace Igraonica.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken ct);
    Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}
