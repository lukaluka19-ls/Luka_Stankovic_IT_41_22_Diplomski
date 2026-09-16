using System.Text.Json;
using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Mappings;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;

namespace Igraonica.Application.Services;

public class TimeSlotService(
    ITimeSlotRepository _timeSlotRepository,
    IAuditLogRepository _auditLog,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _clock,
    ICurrentUserService _currentUser) : ITimeSlotService
{
    public async Task<IReadOnlyList<TimeSlotDto>> GetActiveAsync(CancellationToken ct) =>
        (await _timeSlotRepository.GetActiveAsync(ct)).Select(t => t.ToDto()).ToList();

    public async Task<IReadOnlyList<TimeSlotDto>> GetAllAsync(CancellationToken ct) =>
        (await _timeSlotRepository.GetAllAsync(ct)).Select(t => t.ToDto()).ToList();

    public async Task<TimeSlotDto> CreateAsync(CreateTimeSlotRequest request, CancellationToken ct)
    {
        var timeSlot = TimeSlot.Create(request.StartTime, request.EndTime, request.Name);
        await _timeSlotRepository.AddAsync(timeSlot, ct);

        var json = JsonSerializer.Serialize(request);
        await _auditLog.AddAsync(
            AuditLog.Create(_currentUser.UserId, "CreateTimeSlot", nameof(TimeSlot), json, _clock.UtcNow), ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return timeSlot.ToDto();
    }
}
