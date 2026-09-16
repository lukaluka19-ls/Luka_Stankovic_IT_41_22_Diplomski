using Igraonica.Domain.Common;
using Igraonica.Domain.Enums;

namespace Igraonica.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTimeOffset? PasswordResetTokenExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { } // za EF Core

    public static User Register(string email, string passwordHash, string firstName,
        string lastName, string phoneNumber, string verificationToken, DateTimeOffset now)
    {
        return new User
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Role = UserRole.Client,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            CreatedAt = now,
        };
    }

    public static User CreateAdministrator(string email, string passwordHash, string firstName,
        string lastName, string phoneNumber, DateTimeOffset now)
    {
        return new User
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Role = UserRole.Administrator,
            IsEmailVerified = true,
            CreatedAt = now,
        };
    }

    public void VerifyEmail(string token)
    {
        if (IsEmailVerified)
            return;

        if (EmailVerificationToken is null || EmailVerificationToken != token)
            throw new Exceptions.InvalidTokenException("Token za potvrdu naloga nije ispravan.");

        IsEmailVerified = true;
        EmailVerificationToken = null;
    }

    public void RegenerateVerificationToken(string token)
    {
        if (IsEmailVerified)
            return;

        EmailVerificationToken = token;
    }

    public void RequestPasswordReset(string token, DateTimeOffset expiresAt)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiresAt = expiresAt;
    }

    public void ResetPassword(string token, string newPasswordHash, DateTimeOffset now)
    {
        if (PasswordResetToken is null || PasswordResetToken != token)
            throw new Exceptions.InvalidTokenException("Token za promenu lozinke nije ispravan.");

        if (PasswordResetTokenExpiresAt is null || now > PasswordResetTokenExpiresAt)
            throw new Exceptions.InvalidTokenException("Token za promenu lozinke je istekao.");

        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;
    }

    public string FullName => $"{FirstName} {LastName}";
}
