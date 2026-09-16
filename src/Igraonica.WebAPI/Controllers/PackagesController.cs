using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/packages")]
public class PackagesController(IPackageService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PackageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken ct) => Ok(await service.GetActiveAsync(ct));
}
