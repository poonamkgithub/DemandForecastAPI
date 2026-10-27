using Microsoft.AspNetCore.Mvc;
using DFPlanningApi.DTOs;
using DFPlanningApi.Services;

namespace DFPlanningApi.Controllers;

[ApiController]
[Route("api/demand-forecast")]
public class DemandForecastController : ControllerBase
{
    private static readonly HashSet<string> ValidStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "Draft", "Approved", "Rejected" };

    private readonly IDemandForecastService _service;
    public DemandForecastController(IDemandForecastService service) => _service = service;

    [HttpPost]
    [ProducesResponseType(typeof(DemandForecastResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<DemandForecastResponse>> Create(
        [FromBody] DemandForecastRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<DemandForecastResponse>>> GetAll(
        [FromQuery] string? productCode,
        [FromQuery] string? locationCode,
        CancellationToken cancellationToken)
        => Ok(await _service.GetAllAsync(productCode, locationCode, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DemandForecastResponse>> GetById(
        int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound(new { message = $"Forecast {id} was not found." }) : Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DemandForecastResponse>> Update(
        int id, [FromBody] DemandForecastRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result is null ? NotFound(new { message = $"Forecast {id} was not found." }) : Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<DemandForecastResponse>> ChangeStatus(
        int id, [FromBody] ChangeForecastStatusRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Status) || !ValidStatuses.Contains(request.Status))
            return BadRequest(new { message = "Status must be Draft, Approved, or Rejected." });

        var result = await _service.ChangeStatusAsync(id, request.Status, cancellationToken);
        return result is null ? NotFound(new { message = $"Forecast {id} was not found." }) : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound(new { message = $"Forecast {id} was not found." });
    }
}

public class ChangeForecastStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
