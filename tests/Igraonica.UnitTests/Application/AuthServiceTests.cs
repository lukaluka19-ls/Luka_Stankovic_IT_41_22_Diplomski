using FluentAssertions;
using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Options;
using Igraonica.Application.Services;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Exceptions;
using Igraonica.Domain.Repositories;
using Igraonica.UnitTests.Helpers;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Igraonica.UnitTests.Application;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IOutboxRepository _outbox = Substitute.For<IOutboxRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtService _jwtService = Substitute.For<IJwtService>();
    private readonly FakeDateTimeProvider _clock = new(new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero));

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _userRepository, _refreshTokenRepository, _outbox, _unitOfWork, _hasher, _jwtService, _clock,
            Options.Create(new JwtOptions { Key = "test-key" }),
            Options.Create(new ClientOptions()));
    }

    [Fact]
    public async Task RegisterAsync_odbija_duplikat_adrese()
    {
        _userRepository.EmailExistsAsync("zauzet@example.com", Arg.Any<CancellationToken>()).Returns(true);

        var request = new RegisterRequest("zauzet@example.com", "Lozinka123!", "Ana", "Jovanovic", "0641234567");
        var akcija = async () => await _sut.RegisterAsync(request, CancellationToken.None);

        await akcija.Should().ThrowAsync<DuplicateEmailException>();
    }

    [Fact]
    public async Task LoginAsync_sa_nepostojecim_nalogom_vraca_opstu_poruku()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var request = new LoginRequest("nepostojeci@example.com", "Lozinka123!");
        var akcija = async () => await _sut.LoginAsync(request, CancellationToken.None);

        var exception = await akcija.Should().ThrowAsync<InvalidCredentialsException>();
        exception.Which.Message.Should().Be("Pogresna adresa elektronske poste ili lozinka.");
    }

    [Fact]
    public async Task LoginAsync_sa_pogresnom_lozinkom_vraca_istu_poruku_kao_nepostojeci_nalog()
    {
        var user = User.Register("postoji@example.com", "hash", "Ana", "Jovanovic", "0641234567", "token", _clock.UtcNow);
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var request = new LoginRequest("postoji@example.com", "PogresnaLozinka!");
        var akcija = async () => await _sut.LoginAsync(request, CancellationToken.None);

        var exception = await akcija.Should().ThrowAsync<InvalidCredentialsException>();
        exception.Which.Message.Should().Be("Pogresna adresa elektronske poste ili lozinka.");
    }
}
