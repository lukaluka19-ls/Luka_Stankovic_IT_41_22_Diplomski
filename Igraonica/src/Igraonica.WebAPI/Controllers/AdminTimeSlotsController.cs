using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Igraonica.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/admin/timeslots")]
[Authorize(Roles = nameof(UserRole.Administrator))]
public class AdminTimeSlotsController(ITimeSlotService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TimeSlotDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));

    [HttpPost]
    [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTimeSlotRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
