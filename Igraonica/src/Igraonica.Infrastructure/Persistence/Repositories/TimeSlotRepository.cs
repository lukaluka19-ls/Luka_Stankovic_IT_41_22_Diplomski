using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class TimeSlotRepository(ApplicationDbContext _db) : ITimeSlotRepository
{
    public Task<TimeSlot?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.TimeSlots.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<TimeSlot>> GetActiveAsync(CancellationToken ct) =>
        await _db.TimeSlots.Where(t => t.IsActive).ToListAsync(ct);

    public async Task<IReadOnlyList<TimeSlot>> GetAllAsync(CancellationToken ct) =>
        await _db.TimeSlots.ToListAsync(ct);

    public async Task AddAsync(TimeSlot timeSlot, CancellationToken ct) => await _db.TimeSlots.AddAsync(timeSlot, ct);
}
