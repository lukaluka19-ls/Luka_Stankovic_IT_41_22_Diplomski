using Igraonica.Application.Abstractions;
using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationsController(IReservationService service, ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Client))]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, currentUser.UserId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("my")]
    [Authorize(Roles = nameof(UserRole.Client))]
    [ProducesResponseType(typeof(PagedResultDto<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await service.GetByUserAsync(currentUser.UserId, page, pageSize, ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var isAdmin = currentUser.Role == UserRole.Administrator;
        return Ok(await service.GetByIdAsync(id, currentUser.UserId, isAdmin, ct));
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = nameof(UserRole.Client))]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelReservationRequest request, CancellationToken ct)
    {
        await service.CancelByClientAsync(id, currentUser.UserId, request.Reason, ct);
        return NoContent();
    }
}
