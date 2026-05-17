namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.DegreeLevels.Commands.CreateDegreeLevel;
using AWM.Service.Application.Features.Edu.DegreeLevels.Queries.GetDegreeLevels;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using AWM.Service.WebAPI.Authorization;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing degree levels.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/degree-levels")]
[Produces("application/json")]
public sealed class DegreeLevelsController : BaseController
{
    private readonly ISender _sender;

    public DegreeLevelsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all degree levels with optional filtering.
    /// </summary>
    /// <param name="name">Filter by name (optional, partial match).</param>
    /// <param name="minDurationYears">Filter by minimum duration (optional).</param>
    /// <param name="maxDurationYears">Filter by maximum duration (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of degree levels.</returns>
    [HttpGet]
    [RequireAccess("AcademicPrograms", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DegreeLevelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDegreeLevels(
        [FromQuery] string? name,
        [FromQuery] int? minDurationYears,
        [FromQuery] int? maxDurationYears,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDegreeLevelsQuery
        {
            Name = name,
            MinDurationYears = minDurationYears,
            MaxDurationYears = maxDurationYears
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<DegreeLevelResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Create a new degree level.
    /// </summary>
    /// <param name="request">Degree level creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created degree level ID.</returns>
    [HttpPost]
    [RequireAccess("AcademicPrograms", "Create")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDegreeLevel(
        [FromBody] CreateDegreeLevelRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateDegreeLevelCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(
            nameof(GetDegreeLevels),
            new { name = request.Name },
            result.Value);
    }

    /// <summary>
    /// Update an existing degree level.
    /// </summary>
    /// <param name="id">Degree level ID.</param>
    /// <param name="request">Updated degree level data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [RequireAccess("AcademicPrograms", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDegreeLevel(
        [FromRoute] int id,
        [FromBody] UpdateDegreeLevelRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<AWM.Service.Application.Features.Edu.DegreeLevels.Commands.UpdateDegreeLevel.UpdateDegreeLevelCommand>() with { Id = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete a degree level.
    /// </summary>
    /// <param name="id">Degree level ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [RequireAccess("AcademicPrograms", "Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteDegreeLevel(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var command = new AWM.Service.Application.Features.Edu.DegreeLevels.Commands.DeleteDegreeLevel.DeleteDegreeLevelCommand { Id = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}