namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Directions.Commands.ApproveDirection;
using AWM.Service.Application.Features.Thesis.Directions.Commands.CreateDirection;
using AWM.Service.Application.Features.Thesis.Directions.Commands.RejectDirection;
using AWM.Service.Application.Features.Thesis.Directions.Commands.RequestRevision;
using AWM.Service.Application.Features.Thesis.Directions.Commands.SubmitDirection;
using AWM.Service.Application.Features.Thesis.Directions.Commands.UpdateDirection;
using AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionById;
using AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionsByDepartment;
using AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionsBySupervisor;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing research directions.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class DirectionsController : BaseController
{
    private readonly ISender _sender;

    public DirectionsController(ISender sender)
    {
        _sender = sender;
    }

    #region Queries

    /// <summary>
    /// Get directions by department with optional filtering.
    /// </summary>
    /// <param name="request">Request parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of directions.</returns>
    [HttpGet("by-department")]
    [RequireDepartmentPermission(Permission.Directions_View)]
    [ProducesResponseType(typeof(IReadOnlyList<DirectionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDirectionsByDepartment(
        [FromQuery] GetDirectionsByDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = request.Adapt<GetDirectionsByDepartmentQuery>();

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<DirectionResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get directions by supervisor with optional filtering.
    /// </summary>
    /// <param name="supervisorId">Supervisor ID (required).</param>
    /// <param name="academicYearId">Academic year ID (required).</param>
    /// <param name="workTypeId">Filter by work type ID (optional).</param>
    /// <param name="stateId">Filter by state ID (optional).</param>
    /// <param name="includeDeleted">Include soft-deleted directions (default: false).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of directions.</returns>
    [HttpGet("by-supervisor")]
    [RequirePermission(Permission.Directions_View)]
    [ProducesResponseType(typeof(IReadOnlyList<DirectionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDirectionsBySupervisor(
        [FromQuery] int supervisorId,
        [FromQuery] int academicYearId,
        [FromQuery] int? workTypeId = null,
        [FromQuery] int? stateId = null,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDirectionsBySupervisorQuery
        {
            SupervisorId = supervisorId,
            AcademicYearId = academicYearId,
            WorkTypeId = workTypeId,
            StateId = stateId,
            IncludeDeleted = includeDeleted
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<DirectionResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get direction by ID with full details.
    /// </summary>
    /// <param name="id">Direction ID.</param>
    /// <param name="includeDeleted">Include soft-deleted directions (default: false).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Direction details.</returns>
    [HttpGet("{id}")]
    [RequirePermission(Permission.Directions_View)]
    [ProducesResponseType(typeof(DirectionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDirectionById(
        [FromRoute] long id,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDirectionByIdQuery
        {
            Id = id,
            IncludeDeleted = includeDeleted
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<DirectionDetailResponse>();

        return Ok(response);
    }

    #endregion

    #region Commands - CRUD

    /// <summary>
    /// Create a new research direction.
    /// </summary>
    /// <param name="request">Direction creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created direction ID.</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Directions_Create)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDirection(
        [FromBody] CreateDirectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateDirectionCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(
            nameof(GetDirectionById),
            new { id = result.Value },
            result.Value);
    }

    /// <summary>
    /// Update an existing research direction (only in draft state).
    /// </summary>
    /// <param name="id">Direction ID.</param>
    /// <param name="request">Updated direction data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [RequireDepartmentPermission(Permission.Directions_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDirection(
        [FromRoute] long id,
        [FromBody] UpdateDirectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UpdateDirectionCommand>() with { Id = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    #endregion

    #region Commands - Workflow

    /// <summary>
    /// Submit direction for department review.
    /// </summary>
    /// <param name="id">Direction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id}/submit")]
    [RequirePermission(Permission.Directions_Submit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitDirection(
        [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        var command = new SubmitDirectionCommand
        {
            Id = id
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Approve direction (department action).
    /// </summary>
    /// <param name="id">Direction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id}/approve")]
    [RequireDepartmentPermission(Permission.Directions_Approve)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveDirection(
        [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        var command = new ApproveDirectionCommand
        {
            Id = id
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Reject direction (department action).
    /// </summary>
    /// <param name="id">Direction ID.</param>
    /// <param name="request">Rejection data with optional comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id}/reject")]
    [RequireDepartmentPermission(Permission.Directions_Reject)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RejectDirection(
        [FromRoute] long id,
        [FromBody] RejectDirectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RejectDirectionCommand
        {
            Id = id,
            Comment = request.Comment
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Request revision of direction (department action).
    /// </summary>
    /// <param name="id">Direction ID.</param>
    /// <param name="request">Revision request with required comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id}/request-revision")]
    [RequireDepartmentPermission(Permission.Directions_RequestRevision)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestRevision(
        [FromRoute] long id,
        [FromBody] RequestRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RequestRevisionCommand
        {
            Id = id,
            Comment = request.Comment
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    #endregion
}