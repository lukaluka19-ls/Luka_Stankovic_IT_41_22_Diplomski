using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/admin/packages")]
[Authorize(Roles = nameof(UserRole.Administrator))]
public class AdminPackagesController(IPackageService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PackageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PackageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePackageRequest request, CancellationToken ct) =>
        Ok(await service.UpdateAsync(id, request, ct));
}
