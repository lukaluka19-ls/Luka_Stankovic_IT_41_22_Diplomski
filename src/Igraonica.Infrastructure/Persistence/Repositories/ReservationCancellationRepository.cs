using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class ReservationCancellationRepository(ApplicationDbContext _db) : IReservationCancellationRepository
{
    public async Task AddAsync(ReservationCancellation cancellation, CancellationToken ct) =>
        await _db.ReservationCancellations.AddAsync(cancellation, ct);
}
