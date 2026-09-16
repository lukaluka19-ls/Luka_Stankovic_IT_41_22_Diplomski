using System.Text.Json;
using Igraonica.Application.DTOs;
using Igraonica.Application.Mappings;
using Igraonica.Application.Abstractions;
using Igraonica.Domain.Entities;
using Igraonica.Domain.Exceptions;
using Igraonica.Domain.Repositories;

namespace Igraonica.Application.Services;

public class PackageService(
    IPackageRepository _packageRepository,
    IAuditLogRepository _auditLog,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _clock,
    ICurrentUserService _currentUser) : IPackageService
{
    public async Task<IReadOnlyList<PackageDto>> GetActiveAsync(CancellationToken ct) =>
        (await _packageRepository.GetActivePackagesAsync(ct)).Select(p => p.ToDto()).ToList();

    public async Task<IReadOnlyList<PackageDto>> GetAllAsync(CancellationToken ct) =>
        (await _packageRepository.GetAllAsync(ct)).Select(p => p.ToDto()).ToList();

    public async Task<PackageDto> UpdateAsync(Guid id, UpdatePackageRequest request, CancellationToken ct)
    {
        var package = await _packageRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException("Paket nije pronadjen.");

        package.UpdateDetails(request.Name, request.Description);
        package.UpdatePricing(request.BasePrice, request.PricePerChild);

        if (request.IsActive) package.Activate(); else package.Deactivate();

        var json = JsonSerializer.Serialize(request);
        await _auditLog.AddAsync(
            AuditLog.Create(_currentUser.UserId, "UpdatePackage", nameof(Package), json, _clock.UtcNow), ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return package.ToDto();
    }
}
