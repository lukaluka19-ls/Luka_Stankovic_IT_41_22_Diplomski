using Igraonica.Domain.Entities;
using Igraonica.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Igraonica.Infrastructure.Persistence.Repositories;

public class PackageRepository(ApplicationDbContext _db) : IPackageRepository
{
    public Task<Package?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Packages.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Package?> GetActiveAsync(Guid id, CancellationToken ct) =>
        _db.Packages.FirstOrDefaultAsync(p => p.Id == id && p.IsActive, ct);

    public async Task<IReadOnlyList<Package>> GetActivePackagesAsync(CancellationToken ct) =>
        await _db.Packages.Where(p => p.IsActive).ToListAsync(ct);

    public async Task<IReadOnlyList<Package>> GetAllAsync(CancellationToken ct) =>
        await _db.Packages.ToListAsync(ct);

    public async Task AddAsync(Package package, CancellationToken ct) => await _db.Packages.AddAsync(package, ct);
}
