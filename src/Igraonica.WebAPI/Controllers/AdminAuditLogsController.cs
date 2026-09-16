using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/admin/audit-logs")]
[Authorize(Roles = nameof(UserRole.Administrator))]
public class AdminAuditLogsController(IAdminService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await service.GetAuditLogsAsync(page, pageSize, ct));
}
