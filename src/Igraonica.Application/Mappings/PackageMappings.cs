using Igraonica.Application.DTOs;
using Igraonica.Domain.Entities;

namespace Igraonica.Application.Mappings;

public static class PackageMappings
{
    public static PackageDto ToDto(this Package package) => new(
        package.Id, package.Type.ToString(), package.Name, package.Description,
        package.BasePrice, package.PricePerChild, package.IsActive);
}
