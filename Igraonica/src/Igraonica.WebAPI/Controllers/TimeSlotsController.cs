using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/timeslots")]
public class TimeSlotsController(ITimeSlotService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TimeSlotDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken ct) => Ok(await service.GetActiveAsync(ct));
}
