using Igraonica.Domain.Entities;

namespace Igraonica.Domain.Repositories;

public interface ITimeSlotRepository
{
    Task<TimeSlot?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TimeSlot>> GetActiveAsync(CancellationToken ct);
    Task<IReadOnlyList<TimeSlot>> GetAllAsync(CancellationToken ct);
    Task AddAsync(TimeSlot timeSlot, CancellationToken ct);
}
