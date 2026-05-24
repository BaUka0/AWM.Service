using AWM.Service.Application.Features.Workflow.Applications.Commands.AcceptApplication;
using AWM.Service.Application.Features.Workflow.Applications.Commands.CreateApplication;
using AWM.Service.Application.Features.Workflow.Applications.Commands.RejectApplication;
using AWM.Service.Application.Features.Workflow.Applications.Commands.WithdrawApplication;
using AWM.Service.Application.Features.Workflow.Applications.Queries.GetApplicationsByTopic;
using AWM.Service.Application.Features.Workflow.Applications.Queries.GetMyApplications;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Applications;
using AWM.Service.WebAPI.Common.Contracts.Responses.Applications;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing student applications for topics (Stage 5).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/applications")]
[ApiController]
[Authorize]
public sealed class ApplicationsController : BaseController
{
    private readonly ISender _sender;

    public ApplicationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all applications submitted by the current student.
    /// </summary>
    /// <param name="semesterId">The semester ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of student applications.</returns>
    [HttpGet("my")]
    [RequireAccess("THESIS.APPLICATION", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicApplicationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyApplications([FromQuery] int semesterId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyApplicationsQuery(semesterId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TopicApplicationResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Gets all applications for a specific topic (for supervisor).
    /// </summary>
    /// <param name="topicId">The topic ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of topic applications.</returns>
    [HttpGet("by-topic/{topicId:long}")]
    [RequireAccess("THESIS.APPLICATION", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicApplicationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationsByTopic(long topicId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApplicationsByTopicQuery(topicId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TopicApplicationResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Submits a new application for a topic.
    /// </summary>
    /// <param name="request">The application request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the created application.</returns>
    [HttpPost]
    [RequireAccess("THESIS.APPLICATION", "Create")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateApplicationCommand>();
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Created("", result.Value);
    }

    /// <summary>
    /// Accepts a student application.
    /// </summary>
    /// <param name="id">The application ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{id:long}/accept")]
    [RequireAccess("THESIS.APPLICATION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptApplication(long id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AcceptApplicationCommand(id), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Rejects a student application.
    /// </summary>
    /// <param name="id">The application ID.</param>
    /// <param name="request">The rejection reason.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{id:long}/reject")]
    [RequireAccess("THESIS.APPLICATION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectApplication(long id, [FromBody] RejectApplicationRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RejectApplicationCommand(id, request.Reason), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Withdraws a student application.
    /// </summary>
    /// <param name="id">The application ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:long}")]
    [RequireAccess("THESIS.APPLICATION", "Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> WithdrawApplication(long id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new WithdrawApplicationCommand(id), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
