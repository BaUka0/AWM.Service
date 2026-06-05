using AWM.Service.Application.Features.Workflow.Topics.Commands.CloseTopic;
using AWM.Service.Application.Features.Workflow.Topics.Commands.CompleteTopicReconciliation;
using AWM.Service.Application.Features.Workflow.Topics.Commands.CreateTopic;
using AWM.Service.Application.Features.Workflow.Topics.Commands.MarkTopicsInactive;
using AWM.Service.Application.Features.Workflow.Topics.Commands.ReconcileTopics;
using AWM.Service.Application.Features.Workflow.Topics.Commands.ReviewTopic;
using AWM.Service.Application.Features.Workflow.Topics.Commands.SendTopicsBackForRevision;
using AWM.Service.Application.Features.Workflow.Topics.Commands.SubmitTopics;
using AWM.Service.Application.Features.Workflow.Topics.Commands.UpdateTopic;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetOrgUnitTopics;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetMyTopics;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetTopicById;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetAvailableTopics;
using AWM.Service.Application.Features.Workflow.Topics.Queries.GetReconciliationSummary;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Topics;
using AWM.Service.WebAPI.Common.Contracts.Responses.Topics;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing thesis topics (Stages 4-6).
/// Allows supervisors to propose topics, departments to review and reconcile them.
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
    /// Gets all topics in an org unit for review.
    /// </summary>
    /// <param name="orgUnitId">The org unit (department) ID.</param>
    /// <param name="semesterId">The semester ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of topics in the org unit.</returns>
    [HttpGet("org-unit")]
    [RequireAccess("THESIS.TOPIC", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrgUnitTopics([FromQuery] int orgUnitId, [FromQuery] int semesterId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrgUnitTopicsQuery(orgUnitId, semesterId), cancellationToken);

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

    #region Reconciliation Stage (Согласование тем)

    /// <summary>
    /// Gets the reconciliation summary with aggregate statistics and topic list.
    /// Used by the department during the "Согласование тем" stage.
    /// </summary>
    /// <param name="orgUnitId">The department ID.</param>
    /// <param name="semesterId">The semester ID.</param>
    /// <param name="specialityId">Optional speciality filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Reconciliation summary with topics.</returns>
    [HttpGet("reconciliation")]
    [RequireAccess("THESIS.TOPIC", "Read")]
    [ProducesResponseType(typeof(TopicReconciliationSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReconciliationSummary(
        [FromQuery] int orgUnitId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetReconciliationSummaryQuery(orgUnitId, semesterId, specialityId),
            cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<TopicReconciliationSummaryResponse>();
        return Ok(response);
    }

    /// <summary>
    /// Reconciles (batch final-approves) selected topics.
    /// Transitions topics from Approved/Closed to Reconciled status.
    /// </summary>
    /// <param name="request">The request containing topic IDs to reconcile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("reconcile")]
    [RequireAccess("THESIS.TOPIC", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReconcileTopics(
        [FromBody] ReconcileTopicsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ReconcileTopicsCommand(request.TopicIds),
            cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Marks selected topics as inactive (no students applied).
    /// </summary>
    /// <param name="request">The request containing topic IDs to mark as inactive.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("mark-inactive")]
    [RequireAccess("THESIS.TOPIC", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkTopicsInactive(
        [FromBody] MarkTopicsInactiveRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MarkTopicsInactiveCommand(request.TopicIds),
            cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Sends selected topics back to supervisors for revision.
    /// Typically used for topics with excess applications that need supervisor resolution.
    /// </summary>
    /// <param name="request">The request containing topic IDs and revision comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("send-back-for-revision")]
    [RequireAccess("THESIS.TOPIC", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTopicsBackForRevision(
        [FromBody] SendTopicsBackForRevisionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SendTopicsBackForRevisionCommand(request.TopicIds, request.Comment),
            cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Completes the topic reconciliation stage for a department/semester.
    /// This is an irreversible operation that creates StudentWork entities
    /// for all reconciled topics with their accepted students.
    /// </summary>
    /// <param name="request">The request containing department and semester IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("complete-reconciliation")]
    [RequireAccess("THESIS.TOPIC", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteTopicReconciliation(
        [FromBody] CompleteReconciliationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CompleteTopicReconciliationCommand(request.OrgUnitId, request.SemesterId),
            cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    #endregion
}

