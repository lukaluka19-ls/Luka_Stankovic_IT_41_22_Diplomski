using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext _db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _db.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
    }

    public Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token, ct);

    public Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _db.Users.AnyAsync(u => u.Email == normalized, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct) => await _db.Users.AddAsync(user, ct);
}
