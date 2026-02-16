namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Commands.DegreeLevels.CreateDegreeLevel;
using AWM.Service.Application.Features.Edu.Queries.DegreeLevels.GetDegreeLevels;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing degree levels.
/// </summary>
[ApiController]
[Route("api/v1/degree-levels")]
[Produces("application/json")]
public sealed class DegreeLevelsController : ControllerBase
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

        var response = result.Value
            .Select(dto => new DegreeLevelResponse
            {
                Id = dto.Id,
                Name = dto.Name,
                DurationYears = dto.DurationYears,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastModifiedAt = dto.LastModifiedAt,
                LastModifiedBy = dto.LastModifiedBy
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Create a new degree level.
    /// </summary>
    /// <param name="request">Degree level creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created degree level ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDegreeLevel(
        [FromBody] CreateDegreeLevelRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new CreateDegreeLevelCommand
        {
            Name = request.Name,
            DurationYears = request.DurationYears,
            CreatedBy = currentUserId
        };

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
    /// Handles Result errors and converts them to appropriate HTTP responses.
    /// </summary>
    private IActionResult HandleResultError(KDS.Primitives.FluentResult.Error error)
    {
        return error.Code switch
        {
            "Validation.Error" 
                => BadRequest(new { error.Code, error.Message }),
            
            _ => StatusCode(StatusCodes.Status500InternalServerError, 
                new { error.Code, error.Message })
        };
    }

    /// <summary>
    /// Gets current user ID from authentication context.
    /// TODO: Implement actual user resolution from JWT/Claims.
    /// </summary>
    private int GetCurrentUserId()
    {
        // Placeholder: Extract from User.Claims or ICurrentUserService
        // Example: return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return 1; // Mock user ID for now
    }
}