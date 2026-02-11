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
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing research directions.
/// </summary>
[ApiController]
[Route("api/v1/directions")]
[Produces("application/json")]
public sealed class DirectionsController : ControllerBase
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
    /// <param name="departmentId">Department ID (required).</param>
    /// <param name="academicYearId">Academic year ID (required).</param>
    /// <param name="workTypeId">Filter by work type ID (optional).</param>
    /// <param name="stateId">Filter by state ID (optional).</param>
    /// <param name="supervisorId">Filter by supervisor ID (optional).</param>
    /// <param name="includeDeleted">Include soft-deleted directions (default: false).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of directions.</returns>
    [HttpGet("by-department")]
    [ProducesResponseType(typeof(IReadOnlyList<DirectionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDirectionsByDepartment(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        [FromQuery] int? workTypeId = null,
        [FromQuery] int? stateId = null,
        [FromQuery] int? supervisorId = null,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDirectionsByDepartmentQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId,
            WorkTypeId = workTypeId,
            StateId = stateId,
            SupervisorId = supervisorId,
            IncludeDeleted = includeDeleted
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value
            .Select(dto => new DirectionResponse
            {
                Id = dto.Id,
                DepartmentId = dto.DepartmentId,
                SupervisorId = dto.SupervisorId,
                AcademicYearId = dto.AcademicYearId,
                WorkTypeId = dto.WorkTypeId,
                TitleRu = dto.TitleRu,
                TitleKz = dto.TitleKz,
                TitleEn = dto.TitleEn,
                CurrentStateId = dto.CurrentStateId,
                SubmittedAt = dto.SubmittedAt,
                ReviewedAt = dto.ReviewedAt,
                ReviewedBy = dto.ReviewedBy,
                CreatedAt = dto.CreatedAt,
                IsDeleted = dto.IsDeleted
            })
            .ToList();

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

        var response = result.Value
            .Select(dto => new DirectionResponse
            {
                Id = dto.Id,
                DepartmentId = dto.DepartmentId,
                SupervisorId = dto.SupervisorId,
                AcademicYearId = dto.AcademicYearId,
                WorkTypeId = dto.WorkTypeId,
                TitleRu = dto.TitleRu,
                TitleKz = dto.TitleKz,
                TitleEn = dto.TitleEn,
                CurrentStateId = dto.CurrentStateId,
                SubmittedAt = dto.SubmittedAt,
                ReviewedAt = dto.ReviewedAt,
                ReviewedBy = dto.ReviewedBy,
                CreatedAt = dto.CreatedAt,
                IsDeleted = dto.IsDeleted
            })
            .ToList();

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

        var response = new DirectionDetailResponse
        {
            Id = result.Value.Id,
            DepartmentId = result.Value.DepartmentId,
            SupervisorId = result.Value.SupervisorId,
            AcademicYearId = result.Value.AcademicYearId,
            WorkTypeId = result.Value.WorkTypeId,
            TitleRu = result.Value.TitleRu,
            TitleKz = result.Value.TitleKz,
            TitleEn = result.Value.TitleEn,
            Description = result.Value.Description,
            CurrentStateId = result.Value.CurrentStateId,
            SubmittedAt = result.Value.SubmittedAt,
            ReviewedAt = result.Value.ReviewedAt,
            ReviewedBy = result.Value.ReviewedBy,
            ReviewComment = result.Value.ReviewComment,
            CreatedAt = result.Value.CreatedAt,
            CreatedBy = result.Value.CreatedBy,
            LastModifiedAt = result.Value.LastModifiedAt,
            LastModifiedBy = result.Value.LastModifiedBy,
            IsDeleted = result.Value.IsDeleted,
            DeletedAt = result.Value.DeletedAt,
            DeletedBy = result.Value.DeletedBy,
            TopicsCount = result.Value.TopicsCount
        };

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
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDirection(
        [FromBody] CreateDirectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateDirectionCommand
        {
            DepartmentId = request.DepartmentId,
            SupervisorId = request.SupervisorId,
            AcademicYearId = request.AcademicYearId,
            WorkTypeId = request.WorkTypeId,
            TitleRu = request.TitleRu,
            TitleKz = request.TitleKz,
            TitleEn = request.TitleEn,
            Description = request.Description
        };

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDirection(
        [FromRoute] long id,
        [FromBody] UpdateDirectionRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new UpdateDirectionCommand
        {
            Id = id,
            TitleRu = request.TitleRu,
            TitleKz = request.TitleKz,
            TitleEn = request.TitleEn,
            Description = request.Description,
            ModifiedBy = currentUserId
        };

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitDirection(
        [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new SubmitDirectionCommand
        {
            Id = id,
            SubmittedBy = currentUserId
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveDirection(
        [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new ApproveDirectionCommand
        {
            Id = id,
            ApprovedBy = currentUserId
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RejectDirection(
        [FromRoute] long id,
        [FromBody] RejectDirectionRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new RejectDirectionCommand
        {
            Id = id,
            RejectedBy = currentUserId,
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestRevision(
        [FromRoute] long id,
        [FromBody] RequestRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new RequestRevisionCommand
        {
            Id = id,
            RequestedBy = currentUserId,
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

    #region Helper Methods

    /// <summary>
    /// Handles Result errors and converts them to appropriate HTTP responses.
    /// </summary>
    private IActionResult HandleResultError(KDS.Primitives.FluentResult.Error error)
    {
        return error.Code switch
        {
            "Direction.NotFound" or "Department.NotFound" or "WorkType.NotFound" or "State.NotFound"
                => NotFound(new { error.Code, error.Message }),

            "Direction.Deleted" or "Direction.NotEditable" or "Direction.InvalidState" or "Direction.Unauthorized"
                => BadRequest(new { error.Code, error.Message }),

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

    #endregion
}