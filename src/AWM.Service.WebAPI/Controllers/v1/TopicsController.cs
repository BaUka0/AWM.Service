namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Topics.Commands.ApproveTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.BulkApproveTopics;
using AWM.Service.Application.Features.Thesis.Topics.Commands.CloseTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.CompleteTopicCoordination;
using AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.DeactivateTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.SubmitTopicsForApproval;
using AWM.Service.Application.Features.Thesis.Topics.Commands.UpdateTopic;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicById;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicCoordinationSummary;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsByDirection;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


/// <summary>
/// Controller for managing Thesis Topics.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class TopicsController : BaseController
{
    private readonly ISender _sender;

    public TopicsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a specific topic by ID with full details.
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Topic details with applications</returns>
    [HttpGet("{id}")]
    [RequirePermission(Permission.Topics_View)]
    [ProducesResponseType(typeof(TopicDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var query = new GetTopicByIdQuery { TopicId = id };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<TopicDetailResponse>();

        return Ok(response);
    }

    /// <summary>
    /// Get topics available for student selection.
    /// Both parameters are optional — when omitted, the department and academic year
    /// are auto-resolved from the authenticated user's JWT context.
    /// </summary>
    /// <param name="departmentId">Department ID (optional, auto-resolved from JWT when not provided)</param>
    /// <param name="academicYearId">Academic year ID (optional, uses current active year when not provided)</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of available topics</returns>
    [HttpGet("available")]
    [RequirePermission(Permission.Topics_ViewAvailable)]
    [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] int? departmentId = null,
        [FromQuery] int? academicYearId = null,
        CancellationToken cancellationToken = default)
    {
        // Extract UserId and UniversityId from JWT for auto-resolution
        int? userId = null;
        int? universityId = null;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var universityIdClaim = User.FindFirst("UniversityId");
        if (universityIdClaim != null && int.TryParse(universityIdClaim.Value, out var parsedUniversityId))
        {
            universityId = parsedUniversityId;
        }

        var query = new GetAvailableTopicsQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId,
            UserId = userId,
            UniversityId = universityId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TopicResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get topics by research direction.
    /// </summary>
    /// <param name="directionId">Direction ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of topics linked to the direction</returns>
    [HttpGet("by-direction/{directionId}")]
    [RequirePermission(Permission.Topics_View)]
    [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByDirection(long directionId, CancellationToken cancellationToken = default)
    {
        var query = new GetTopicsByDirectionQuery { DirectionId = directionId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TopicResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Create a new thesis topic.
    /// </summary>
    /// <param name="request">Create topic request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created topic ID</returns>
    [HttpPost]
    [RequirePermission(Permission.Topics_Create)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTopicRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateTopicCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update an existing thesis topic.
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <param name="request">Update topic request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [RequirePermission(Permission.Topics_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateTopicRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UpdateTopicCommand>() with { TopicId = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Approve a thesis topic (department action).
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/approve")]
    [RequireDepartmentPermission(Permission.Topics_Approve)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Approve(long id, CancellationToken cancellationToken = default)
    {
        var command = new ApproveTopicCommand
        {
            TopicId = id,
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Close a thesis topic (no more applications accepted).
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/close")]
    [RequirePermission(Permission.Topics_Close)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Close(long id, CancellationToken cancellationToken = default)
    {
        var command = new CloseTopicCommand
        {
            TopicId = id,
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Submit topics for department approval (batch operation by supervisor).
    /// </summary>
    /// <param name="request">List of topic IDs to submit</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPost("submit-for-approval")]
    [RequirePermission(Permission.Topics_Create)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitForApproval(
        [FromBody] SubmitTopicsForApprovalRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<SubmitTopicsForApprovalCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Bulk approve topics (department action).
    /// </summary>
    /// <param name="request">List of topic IDs to approve</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPost("bulk-approve")]
    [RequireDepartmentPermission(Permission.Topics_Approve)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkApprove(
        [FromBody] BulkApproveTopicsRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<BulkApproveTopicsCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Get topic coordination summary for a department (department action).
    /// Shows statistics: total topics, accepted applications, topics without students, etc.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Coordination summary</returns>
    [HttpGet("coordination-summary")]
    [RequireDepartmentPermission(Permission.Topics_View)]
    [ProducesResponseType(typeof(TopicCoordinationSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCoordinationSummary(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTopicCoordinationSummaryQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<TopicCoordinationSummaryResponse>();
        return Ok(response);
    }

    /// <summary>
    /// Deactivate a topic without students (department action).
    /// </summary>
    /// <param name="id">Topic ID to deactivate</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/deactivate")]
    [RequireDepartmentPermission(Permission.Topics_Close)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken = default)
    {
        var command = new DeactivateTopicCommand { TopicId = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Complete topic coordination for a department (department action).
    /// Closes all open topics, rejects pending applications, and notifies participants.
    /// </summary>
    /// <param name="request">Coordination completion request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPost("complete-coordination")]
    [RequireDepartmentPermission(Permission.Topics_Approve)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteCoordination(
        [FromBody] CompleteTopicCoordinationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CompleteTopicCoordinationCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}