using Igraonica.Application.DTOs;

namespace Igraonica.Application.Services;

public interface IAvailabilityService
{
    Task<IReadOnlyList<AvailabilityDayDto>> GetAvailabilityAsync(DateOnly from, DateOnly to, CancellationToken ct);
}
