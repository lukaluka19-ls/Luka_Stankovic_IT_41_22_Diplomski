namespace Igraonica.Application.DTOs;

public sealed record PackageDto(
    Guid Id, string Type, string Name, string Description,
    decimal BasePrice, decimal PricePerChild, bool IsActive);

public sealed record UpdatePackageRequest(
    string Name, string Description, decimal BasePrice, decimal PricePerChild, bool IsActive);
