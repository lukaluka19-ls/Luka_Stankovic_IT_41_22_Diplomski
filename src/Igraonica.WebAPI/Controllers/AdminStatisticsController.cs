using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/admin/statistics")]
[Authorize(Roles = nameof(UserRole.Administrator))]
public class AdminStatisticsController(IAdminService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(StatisticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await service.GetStatisticsAsync(ct));
}
