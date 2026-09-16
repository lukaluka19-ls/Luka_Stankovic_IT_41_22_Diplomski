using Igraonica.Domain.Entities;

namespace Igraonica.Domain.Repositories;

public interface IReservationCancellationRepository
{
    Task AddAsync(ReservationCancellation cancellation, CancellationToken ct);
}
