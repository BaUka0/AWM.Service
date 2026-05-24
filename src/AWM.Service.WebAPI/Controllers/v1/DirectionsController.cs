using AWM.Service.Application.Features.Workflow.Directions.Commands.CreateDirection;
using AWM.Service.Application.Features.Workflow.Directions.Commands.ReviewDirection;
using AWM.Service.Application.Features.Workflow.Directions.Commands.SubmitDirection;
using AWM.Service.Application.Features.Workflow.Directions.Commands.UpdateDirection;
using AWM.Service.Application.Features.Workflow.Directions.Queries.GetDepartmentDirections;
using AWM.Service.Application.Features.Workflow.Directions.Queries.GetDirectionById;
using AWM.Service.Application.Features.Workflow.Directions.Queries.GetMyDirections;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Directions;
using AWM.Service.WebAPI.Common.Contracts.Responses.Directions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing workflow directions.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/directions")]
[ApiController]
public sealed class DirectionsController : BaseController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionsController"/> class.
    /// </summary>
    public DirectionsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets directions created by the current user (Supervisor).
    /// </summary>
    [HttpGet("my")]
    [RequireAccess("THESIS.DIRECTION", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DirectionSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDirections(
        [FromQuery] int? semesterId,
        CancellationToken cancellationToken)
    {
        var query = new GetMyDirectionsQuery(semesterId);
        var result = await _sender.Send(query, cancellationToken);
        
        if (result.IsFailed) return HandleResultError(result.Error);
        
        var response = result.Value.Adapt<IReadOnlyList<DirectionSummaryResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Gets a specific direction by identifier.
    /// </summary>
    [HttpGet("{id}")]
    [RequireAccess("THESIS.DIRECTION", "Read")]
    [ProducesResponseType(typeof(DirectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDirectionById(
        long id,
        CancellationToken cancellationToken)
    {
        var query = new GetDirectionByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        
        if (result.IsFailed) return HandleResultError(result.Error);
        
        var response = result.Value.Adapt<DirectionResponse>();
        return Ok(response);
    }

    /// <summary>
    /// Gets directions for a specific department.
    /// </summary>
    [HttpGet("department/{orgUnitId}")]
    [RequireAccess("THESIS.DIRECTION", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DirectionSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartmentDirections(
        int orgUnitId,
        [FromQuery] int? semesterId,
        [FromQuery] int? stateId,
        CancellationToken cancellationToken)
    {
        var query = new GetDepartmentDirectionsQuery(orgUnitId, semesterId, stateId);
        var result = await _sender.Send(query, cancellationToken);
        
        if (result.IsFailed) return HandleResultError(result.Error);
        
        var response = result.Value.Adapt<IReadOnlyList<DirectionSummaryResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Creates a new direction.
    /// </summary>
    [HttpPost]
    [RequireAccess("THESIS.DIRECTION", "Create")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDirection(
        [FromBody] CreateDirectionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDirectionCommand(
            request.SemesterId,
            request.WorkTypeId,
            request.TitleRu,
            request.TitleKz,
            request.TitleEn,
            request.DescriptionRu,
            request.DescriptionKz,
            request.DescriptionEn);

        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed) return HandleResultError(result.Error);
        
        return CreatedAtAction(nameof(GetDirectionById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Updates an existing direction in Draft state.
    /// </summary>
    [HttpPut("{id}")]
    [RequireAccess("THESIS.DIRECTION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDirection(
        long id,
        [FromBody] UpdateDirectionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDirectionCommand(
            id,
            request.TitleRu,
            request.TitleKz,
            request.TitleEn,
            request.DescriptionRu,
            request.DescriptionKz,
            request.DescriptionEn);

        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed) return HandleResultError(result.Error);
        
        return Ok();
    }

    /// <summary>
    /// Submits a direction for review.
    /// </summary>
    [HttpPost("{id}/submit")]
    [RequireAccess("THESIS.DIRECTION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitDirection(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new SubmitDirectionCommand(id);
        
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed) return HandleResultError(result.Error);
        
        return Ok();
    }

    /// <summary>
    /// Reviews a submitted direction (Department action).
    /// </summary>
    [HttpPost("{id}/review")]
    [RequireAccess("THESIS.DIRECTION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReviewDirection(
        long id,
        [FromBody] ReviewDirectionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReviewDirectionCommand(
            id,
            (ReviewDecision)request.DecisionId,
            request.Comment);
            
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed) return HandleResultError(result.Error);
        
        return Ok();
    }
}
