using Igraonica.Application.DTOs;

namespace Igraonica.Application.Services;

public interface ITimeSlotService
{
    Task<IReadOnlyList<TimeSlotDto>> GetActiveAsync(CancellationToken ct);
    Task<IReadOnlyList<TimeSlotDto>> GetAllAsync(CancellationToken ct);
    Task<TimeSlotDto> CreateAsync(CreateTimeSlotRequest request, CancellationToken ct);
}
