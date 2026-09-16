using FluentAssertions;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Exceptions;
using Xunit;

namespace Igraonica.UnitTests.Domain;

public class UserTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void VerifyEmail_sa_ispravnim_tokenom_potvrdjuje_nalog()
    {
        var user = User.Register("test@example.com", "hash", "Ana", "Jovanovic", "0641234567", "token-abc", Now);

        user.VerifyEmail("token-abc");

        user.IsEmailVerified.Should().BeTrue();
    }

    [Fact]
    public void VerifyEmail_sa_pogresnim_tokenom_baca_izuzetak()
    {
        var user = User.Register("test@example.com", "hash", "Ana", "Jovanovic", "0641234567", "token-abc", Now);

        var akcija = () => user.VerifyEmail("pogresan-token");

        akcija.Should().Throw<InvalidTokenException>();
        user.IsEmailVerified.Should().BeFalse();
    }

    [Fact]
    public void ResetPassword_sa_isteklim_tokenom_baca_izuzetak()
    {
        var user = User.Register("test@example.com", "hash", "Ana", "Jovanovic", "0641234567", "token-abc", Now);
        user.VerifyEmail("token-abc");
        user.RequestPasswordReset("reset-token", Now.AddHours(1));

        var akcija = () => user.ResetPassword("reset-token", "novi-hash", Now.AddHours(2));

        akcija.Should().Throw<InvalidTokenException>();
    }

    [Fact]
    public void ResetPassword_pre_isteka_menja_lozinku()
    {
        var user = User.Register("test@example.com", "hash", "Ana", "Jovanovic", "0641234567", "token-abc", Now);
        user.RequestPasswordReset("reset-token", Now.AddHours(1));

        user.ResetPassword("reset-token", "novi-hash", Now.AddMinutes(30));

        user.PasswordHash.Should().Be("novi-hash");
    }
}
