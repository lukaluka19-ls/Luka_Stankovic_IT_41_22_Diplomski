using Igraonica.Domain.Common;

namespace Igraonica.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTimeOffset now, int lifetimeDays)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            CreatedAt = now,
            ExpiresAt = now.AddDays(lifetimeDays),
        };
    }

    public bool IsActive(DateTimeOffset now) => RevokedAt is null && now < ExpiresAt;

    public void Revoke(DateTimeOffset now) => RevokedAt = now;
}
