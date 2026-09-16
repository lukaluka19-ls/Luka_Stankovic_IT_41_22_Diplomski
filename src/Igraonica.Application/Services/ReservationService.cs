using System.Data;
using System.Text.Json;
using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Emails;
using Igraonica.Application.Mappings;
using Igraonica.Application.Options;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Enums;
using Igraonica.Domain.Exceptions;
using Igraonica.Domain.Repositories;
using Igraonica.Domain.Rules;
using Microsoft.Extensions.Options;

namespace Igraonica.Application.Services;

public class ReservationService(
    IReservationRepository _reservationRepository,
    IReservationCancellationRepository _cancellationRepository,
    IPackageRepository _packageRepository,
    ITimeSlotRepository _timeSlotRepository,
    IUserRepository _userRepository,
    IOutboxRepository _outbox,
    IAuditLogRepository _auditLog,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _clock,
    IReservationNumberGenerator _numberGenerator,
    IOptions<BusinessRulesOptions> _rulesOptions) : IReservationService
{
    private BusinessRulesOptions Rules => _rulesOptions.Value;

    public async Task<ReservationDto> CreateAsync(CreateReservationRequest request, Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new EntityNotFoundException("Korisnik nije pronadjen.");

        if (!user.IsEmailVerified)
            throw new EmailNotVerifiedException(
                "Rezervaciju moze da kreira samo korisnik koji je potvrdio svoju adresu elektronske poste.");

        var package = await _packageRepository.GetActiveAsync(request.PackageId, ct)
            ?? throw new EntityNotFoundException("Izabrani paket nije dostupan.");

        var timeSlot = await _timeSlotRepository.GetByIdAsync(request.TimeSlotId, ct)
            ?? throw new EntityNotFoundException("Izabrani termin nije pronadjen.");

        // Nivo izolacije Serializable je neophodan: bez njega dva istovremena
        // zahteva za isti datum mogu oba proci proveru kapaciteta pre nego sto
        // bilo koji od njih upise podatak (problem istovremenog pristupa).
        await using var tx = await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, ct);

        var existing = await _reservationRepository.CountActiveByDateAsync(request.EventDate, ct);

        CapacityRules.EnsureCapacityAvailable(request.EventDate, existing, Rules.WeekdayCapacity, Rules.WeekendCapacity);

        var reservation = Reservation.Create(userId, request.ChildName, request.ChildAge,
            request.EventDate, request.TimeSlotId, request.NumberOfChildren, package,
            _clock.UtcNow, Rules.ConfirmationDeadlineDays);

        reservation.AssignNumber(await _numberGenerator.NextAsync(ct));

        await _reservationRepository.AddAsync(reservation, ct);

        var (subject, body) = EmailTemplates.ReservationCreated(reservation);
        await _outbox.AddAsync(
            OutboxMessage.Create(OutboxMessageType.ReservationCreated, user.Email, subject, body, _clock.UtcNow), ct);

        await AddAuditLogAsync(userId, "CreateReservation", reservation.Id, new
        {
            reservation.ReservationNumber, reservation.EventDate, reservation.TotalPrice,
        }, ct);

        await _unitOfWork.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return reservation.ToDto(package, timeSlot, _clock.UtcNow);
    }

    public async Task CancelByClientAsync(Guid id, Guid userId, string reason, CancellationToken ct)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException("Rezervacija nije pronadjena.");

        if (reservation.UserId != userId)
            throw new ForbiddenException("Nemate pravo pristupa ovoj rezervaciji.");

        reservation.CancelByClient(reason, _clock.UtcNow, Rules.CancellationNoticeDays);

        var user = await _userRepository.GetByIdAsync(userId, ct);

        await SaveCancellationAsync(reservation, reason, userId, ct);

        if (user is not null)
        {
            var (subject, body) = EmailTemplates.ReservationCancelled(reservation, reason);
            await _outbox.AddAsync(
                OutboxMessage.Create(OutboxMessageType.ReservationCancelled, user.Email, subject, body, _clock.UtcNow), ct);
        }

        await AddAuditLogAsync(userId, "CancelReservationByClient", reservation.Id, new { reason }, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task CancelByAdminAsync(Guid id, string? reason, CancellationToken ct)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException("Rezervacija nije pronadjena.");

        reservation.CancelByAdmin(_clock.UtcNow);

        var effectiveReason = string.IsNullOrWhiteSpace(reason)
            ? "Rezervacija je otkazana od strane administratora igraonice."
            : reason;

        var user = await _userRepository.GetByIdAsync(reservation.UserId, ct);
        await SaveCancellationAsync(reservation, effectiveReason, reservation.UserId, ct);

        if (user is not null)
        {
            var (subject, body) = EmailTemplates.ReservationCancelled(reservation, effectiveReason);
            await _outbox.AddAsync(
                OutboxMessage.Create(OutboxMessageType.ReservationCancelled, user.Email, subject, body, _clock.UtcNow), ct);
        }

        await AddAuditLogAsync(reservation.UserId, "CancelReservationByAdmin", reservation.Id,
            new { reason = effectiveReason }, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<ReservationDto> ConfirmArrivalAsync(Guid id, CancellationToken ct)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException("Rezervacija nije pronadjena.");

        reservation.ConfirmArrival(_clock.UtcNow);

        var user = await _userRepository.GetByIdAsync(reservation.UserId, ct);
        if (user is not null)
        {
            var (subject, body) = EmailTemplates.ReservationConfirmed(reservation);
            await _outbox.AddAsync(
                OutboxMessage.Create(OutboxMessageType.ReservationConfirmed, user.Email, subject, body, _clock.UtcNow), ct);
        }

        await AddAuditLogAsync(reservation.UserId, "ConfirmReservationArrival", reservation.Id, new { }, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        var package = await _packageRepository.GetByIdAsync(reservation.PackageId, ct);
        var timeSlot = await _timeSlotRepository.GetByIdAsync(reservation.TimeSlotId, ct);
        return reservation.ToDto(package!, timeSlot!, _clock.UtcNow);
    }

    public async Task<ReservationDto> GetByIdAsync(Guid id, Guid requestingUserId, bool isAdmin, CancellationToken ct)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException("Rezervacija nije pronadjena.");

        if (!isAdmin && reservation.UserId != requestingUserId)
            throw new ForbiddenException("Nemate pravo pristupa ovoj rezervaciji.");

        var package = await _packageRepository.GetByIdAsync(reservation.PackageId, ct);
        var timeSlot = await _timeSlotRepository.GetByIdAsync(reservation.TimeSlotId, ct);
        return reservation.ToDto(package!, timeSlot!, _clock.UtcNow);
    }

    public async Task<PagedResultDto<ReservationDto>> GetByUserAsync(
        Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var result = await _reservationRepository.GetByUserAsync(userId, page, pageSize, ct);
        return await MapPagedAsync(result, ct);
    }

    public async Task<PagedResultDto<ReservationDto>> GetFilteredAsync(
        AdminReservationFilterQuery query, CancellationToken ct)
    {
        ReservationStatus? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<ReservationStatus>(query.Status, true, out var parsed))
        {
            status = parsed;
        }

        var filter = new ReservationFilter(status, query.FromDate, query.ToDate, query.UserId,
            query.Page, query.PageSize);

        var result = await _reservationRepository.GetFilteredAsync(filter, ct);
        return await MapPagedAsync(result, ct);
    }

    public async Task<int> ExpireOverdueReservationsAsync(CancellationToken ct)
    {
        var due = await _reservationRepository.GetDueForExpirationAsync(_clock.UtcNow, ct);
        var count = 0;

        foreach (var reservation in due)
        {
            try
            {
                reservation.AutoExpire(_clock.UtcNow);

                var user = await _userRepository.GetByIdAsync(reservation.UserId, ct);
                if (user is not null)
                {
                    var (subject, body) = EmailTemplates.ReservationAutoExpired(reservation);
                    await _outbox.AddAsync(OutboxMessage.Create(
                        OutboxMessageType.ReservationAutoExpired, user.Email, subject, body, _clock.UtcNow), ct);
                }

                await AddAuditLogAsync(reservation.UserId, "AutoExpireReservation", reservation.Id, new { }, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                count++;
            }
            catch (DomainException)
            {
                // Idempotentnost: ako je u medjuvremenu vec obradjena, preskace se.
            }
        }

        return count;
    }

    public async Task<int> CompleteOverdueReservationsAsync(CancellationToken ct)
    {
        var due = await _reservationRepository.GetDueForCompletionAsync(_clock.UtcNow, ct);
        var count = 0;

        foreach (var reservation in due)
        {
            try
            {
                reservation.Complete(_clock.UtcNow);
                await _unitOfWork.SaveChangesAsync(ct);
                count++;
            }
            catch (DomainException)
            {
            }
        }

        return count;
    }

    private async Task SaveCancellationAsync(Reservation reservation, string reason, Guid byUserId, CancellationToken ct)
    {
        var cancellation = ReservationCancellation.Create(reservation.Id, reason, _clock.UtcNow, byUserId);
        await _cancellationRepository.AddAsync(cancellation, ct);
    }

    private async Task<PagedResultDto<ReservationDto>> MapPagedAsync(
        PagedResult<Reservation> result, CancellationToken ct)
    {
        var packages = (await _packageRepository.GetAllAsync(ct)).ToDictionary(p => p.Id);
        var timeSlots = (await _timeSlotRepository.GetAllAsync(ct)).ToDictionary(t => t.Id);

        var items = result.Items
            .Select(r => r.ToDto(packages[r.PackageId], timeSlots[r.TimeSlotId], _clock.UtcNow))
            .ToList();

        return new PagedResultDto<ReservationDto>(items, result.TotalCount, result.Page, result.PageSize);
    }

    private async Task AddAuditLogAsync(Guid userId, string action, Guid entityId, object details, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(new { entityId, data = details });
        await _auditLog.AddAsync(AuditLog.Create(userId, action, nameof(Reservation), json, _clock.UtcNow), ct);
    }
}
