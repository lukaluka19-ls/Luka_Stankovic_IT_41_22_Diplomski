using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/admin/reservations")]
[Authorize(Roles = nameof(UserRole.Administrator))]
public class AdminReservationsController(IReservationService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdminReservationFilterQuery query, CancellationToken ct) =>
        Ok(await service.GetFilteredAsync(query, ct));

    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct) =>
        Ok(await service.ConfirmArrivalAsync(id, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] string? reason, CancellationToken ct)
    {
        await service.CancelByAdminAsync(id, reason, ct);
        return NoContent();
    }
}
