using AWM.Service.Application.Features.Workflow.Topics.Commands.CloseTopic;
using AWM.Service.Application.Features.Workflow.Topics.Commands.CreateTopic;
using AWM.Service.Application.Features.Workflow.Topics.Commands.ReviewTopic;
using AWM.Service.Application.Features.Workflow.Topics.Commands.SubmitTopics;
using AWM.Service.Application.Features.Workflow.Topics.Commands.UpdateTopic;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetDepartmentTopics;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetMyTopics;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetTopicById;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetAvailableTopics;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Topics;
using AWM.Service.WebAPI.Common.Contracts.Responses.Topics;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing thesis topics (Stage 4).
/// Allows supervisors to propose topics and departments to review them.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/topics")]
[ApiController]
[Authorize]
public sealed class TopicsController : BaseController
{
    private readonly ISender _sender;

    public TopicsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets topics created by the current supervisor for a specific semester.
    /// </summary>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of topics.</returns>
    [HttpGet("my")]
    [RequireAccess("THESIS.TOPIC", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTopics([FromQuery] int semesterId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyTopicsQuery(semesterId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TopicResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Gets detailed information about a specific topic.
    /// </summary>
    /// <param name="id">The topic ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Topic details.</returns>
    [HttpGet("{id:long}")]
    [RequireAccess("THESIS.TOPIC", "Read")]
    [ProducesResponseType(typeof(TopicDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTopicById(long id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTopicByIdQuery(id), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<TopicDetailResponse>();
        return Ok(response);
    }

    /// <summary>
    /// Creates a new thesis topic.
    /// </summary>
    /// <param name="request">The topic creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the created topic.</returns>
    [HttpPost]
    [RequireAccess("THESIS.TOPIC", "Create")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateTopicCommand>();
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetTopicById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Updates an existing topic.
    /// </summary>
    /// <param name="id">The topic ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id:long}")]
    [RequireAccess("THESIS.TOPIC", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTopic(long id, [FromBody] UpdateTopicRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateTopicCommand>() with { Id = id };
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Submits a batch of topics for department review.
    /// </summary>
    /// <param name="request">The request containing list of topic IDs to submit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("submit")]
    [RequireAccess("THESIS.TOPIC", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitTopics([FromBody] SubmitTopicsRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new SubmitTopicsCommand(request.TopicIds), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Reviews a topic (Approve or Reject).
    /// </summary>
    /// <param name="id">The topic ID.</param>
    /// <param name="request">The review decision and optional comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{id:long}/review")]
    [RequireAccess("THESIS.TOPIC", "Update")] // Usually department role
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReviewTopic(long id, [FromBody] ReviewTopicRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<ReviewTopicCommand>() with { TopicId = id };
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets all topics in a department for review.
    /// </summary>
    /// <param name="orgUnitId">The department ID.</param>
    /// <param name="semesterId">The semester ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of topics in the department.</returns>
    [HttpGet("department")]
    [RequireAccess("THESIS.TOPIC", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartmentTopics([FromQuery] int orgUnitId, [FromQuery] int semesterId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDepartmentTopicsQuery(orgUnitId, semesterId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TopicResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Gets available approved topics for student selection.
    /// </summary>
    /// <param name="orgUnitId">The department ID.</param>
    /// <param name="semesterId">The semester ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of available topics.</returns>
    [HttpGet("available")]
    [RequireAccess("THESIS.TOPIC", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableTopics([FromQuery] int orgUnitId, [FromQuery] int semesterId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAvailableTopicsQuery(orgUnitId, semesterId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TopicResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Closes a topic, preventing new applications.
    /// Only the topic creator can close their own approved topics.
    /// </summary>
    /// <param name="id">The topic ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id:long}/close")]
    [RequireAccess("THESIS.TOPIC", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseTopic(long id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CloseTopicCommand(id), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
