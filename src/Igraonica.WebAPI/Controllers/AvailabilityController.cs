using Igraonica.Application.DTOs;
using Igraonica.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Controllers;

[ApiController]
[Route("api/availability")]
public class AvailabilityController(IAvailabilityService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AvailabilityDayDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken ct) =>
        Ok(await service.GetAvailabilityAsync(from, to, ct));
}
