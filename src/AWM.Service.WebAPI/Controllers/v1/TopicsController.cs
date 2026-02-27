namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Topics.Commands.ApproveTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.CloseTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.UpdateTopic;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicById;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsByDirection;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of available topics</returns>
    [HttpGet("available")]
    [RequirePermission(Permission.Topics_ViewAvailable)]
    [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAvailableTopicsQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
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
}