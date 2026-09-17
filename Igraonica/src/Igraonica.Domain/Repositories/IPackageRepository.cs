using Igraonica.Domain.Entities;

namespace Igraonica.Domain.Repositories;

public interface IPackageRepository
{
    Task<Package?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Package?> GetActiveAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Package>> GetActivePackagesAsync(CancellationToken ct);
    Task<IReadOnlyList<Package>> GetAllAsync(CancellationToken ct);
    Task AddAsync(Package package, CancellationToken ct);
}
