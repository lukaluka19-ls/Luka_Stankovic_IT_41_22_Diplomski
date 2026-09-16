using System.Data;
using FluentAssertions;
using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Options;
using Igraonica.Application.Services;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Exceptions;
using Igraonica.Domain.Repositories;
using Igraonica.UnitTests.Helpers;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Igraonica.UnitTests.Application;

public class ReservationServiceTests
{
    private readonly IReservationRepository _reservationRepository = Substitute.For<IReservationRepository>();
    private readonly IReservationCancellationRepository _cancellationRepository =
        Substitute.For<IReservationCancellationRepository>();
    private readonly IPackageRepository _packageRepository = Substitute.For<IPackageRepository>();
    private readonly ITimeSlotRepository _timeSlotRepository = Substitute.For<ITimeSlotRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IOutboxRepository _outbox = Substitute.For<IOutboxRepository>();
    private readonly IAuditLogRepository _auditLog = Substitute.For<IAuditLogRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IReservationNumberGenerator _numberGenerator = Substitute.For<IReservationNumberGenerator>();
    private readonly FakeDateTimeProvider _clock = new(new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero));

    private readonly ReservationService _sut;

    public ReservationServiceTests()
    {
        var rules = new BusinessRulesOptions();
        _unitOfWork.BeginTransactionAsync(Arg.Any<IsolationLevel>(), Arg.Any<CancellationToken>())
            .Returns(Substitute.For<IDbContextTransactionHandle>());
        _numberGenerator.NextAsync(Arg.Any<CancellationToken>()).Returns("REZ-2026-00001");

        _sut = new ReservationService(
            _reservationRepository, _cancellationRepository, _packageRepository, _timeSlotRepository,
            _userRepository, _outbox, _auditLog, _unitOfWork, _clock, _numberGenerator,
            Options.Create(rules));
    }

    // T-10: rezervacija neverifikovanog korisnika - odbijena (PP-07)
    [Fact]
    public async Task CreateAsync_odbija_neverifikovanog_korisnika()
    {
        var user = User.Register("test@example.com", "hash", "Ana", "Jovanovic", "0641234567", "token", _clock.UtcNow);
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(user);

        var request = new CreateReservationRequest(
            "Marko", 6, DateOnly.FromDateTime(_clock.UtcNow.Date).AddDays(30), Guid.NewGuid(), 10, Guid.NewGuid());

        var akcija = async () => await _sut.CreateAsync(request, Guid.NewGuid(), CancellationToken.None);

        await akcija.Should().ThrowAsync<EmailNotVerifiedException>();
    }

    [Fact]
    public async Task CreateAsync_odbija_kada_je_kapacitet_popunjen()
    {
        var user = User.Register("test@example.com", "hash", "Ana", "Jovanovic", "0641234567", "token", _clock.UtcNow);
        user.VerifyEmail("token");
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(user);

        var package = TestFactory().Package;
        _packageRepository.GetActiveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(package);
        _timeSlotRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(TimeSlot.Create(new TimeOnly(10, 0), new TimeOnly(12, 0), "Prepodne"));

        var ponedeljak = DateOnly.FromDateTime(_clock.UtcNow.Date).AddDays(30);
        _reservationRepository.CountActiveByDateAsync(ponedeljak, Arg.Any<CancellationToken>()).Returns(1);

        var request = new CreateReservationRequest("Marko", 6, ponedeljak, Guid.NewGuid(), 10, Guid.NewGuid());

        var akcija = async () => await _sut.CreateAsync(request, Guid.NewGuid(), CancellationToken.None);

        await akcija.Should().ThrowAsync<CapacityExceededException>();
    }

    private (Package Package,
        Guid Id) TestFactory()
    {
        var package = Package.Create(PackageType.Basic, "Osnovni paket", "Opis", 9000, 900);
        return (package, package.Id);
    }
}
