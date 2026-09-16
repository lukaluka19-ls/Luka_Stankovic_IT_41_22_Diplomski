using Igraonica.Application.DTOs;

namespace Igraonica.Application.Services;

public interface IPackageService
{
    Task<IReadOnlyList<PackageDto>> GetActiveAsync(CancellationToken ct);
    Task<IReadOnlyList<PackageDto>> GetAllAsync(CancellationToken ct);
    Task<PackageDto> UpdateAsync(Guid id, UpdatePackageRequest request, CancellationToken ct);
}
