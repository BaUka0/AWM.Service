namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Applications.Commands.AcceptApplication;
using AWM.Service.Application.Features.Thesis.Applications.Commands.CreateApplication;
using AWM.Service.Application.Features.Thesis.Applications.Commands.RejectApplication;
using AWM.Service.Application.Features.Thesis.Applications.Commands.WithdrawApplication;
using AWM.Service.Application.Features.Thesis.Applications.DTOs;
using AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByStudent;
using AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByTopic;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing topic applications (student applies → supervisor reviews).
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/applications")]
[Produces("application/json")]
public class TopicApplicationsController : BaseController
{
    private readonly ISender _sender;

    public TopicApplicationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Submit a new application for a topic (student action).
    /// </summary>
    /// <param name="request">Application details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created application ID</returns>
    [HttpPost]
    [RequirePermission(Permission.Applications_Create)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateApplicationCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetByTopic), new { topicId = request.TopicId }, result.Value);
    }

    /// <summary>
    /// Get all applications for a topic (supervisor action).
    /// </summary>
    /// <param name="topicId">Topic ID</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of applications for the topic</returns>
    [HttpGet("by-topic/{topicId:long}")]
    [RequirePermission(Permission.Applications_View)]
    [ProducesResponseType(typeof(IReadOnlyList<TopicApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByTopic(
        long topicId,
        [FromQuery] ApplicationStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetApplicationsByTopicQuery
        {
            TopicId = topicId,
            StatusFilter = status.HasValue ? (int)status.Value : null
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<TopicApplicationResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Get all applications for the current student ("My Applications").
    /// </summary>
    /// <param name="academicYearId">Optional academic year filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of student's applications</returns>
    [HttpGet("my")]
    [RequirePermission(Permission.Applications_ViewOwn)]
    [ProducesResponseType(typeof(IReadOnlyList<TopicApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyApplications(
        [FromQuery] int? academicYearId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetApplicationsByStudentQuery
        {
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<TopicApplicationResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Accept an application (supervisor action).
    /// </summary>
    /// <param name="applicationId">Application ID to accept</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{applicationId:long}/accept")]
    [RequirePermission(Permission.Applications_Accept)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Accept(
        long applicationId,
        CancellationToken cancellationToken = default)
    {
        var command = new AcceptApplicationCommand
        {
            ApplicationId = applicationId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Reject an application (supervisor action).
    /// </summary>
    /// <param name="applicationId">Application ID to reject</param>
    /// <param name="request">Rejection details with reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{applicationId:long}/reject")]
    [RequirePermission(Permission.Applications_Reject)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Reject(
        long applicationId,
        [FromBody] RejectApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<RejectApplicationCommand>() with { ApplicationId = applicationId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Withdraw an application (student action - soft delete).
    /// </summary>
    /// <param name="applicationId">Application ID to withdraw</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{applicationId:long}")]
    [RequirePermission(Permission.Applications_Withdraw)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Withdraw(
        long applicationId,
        CancellationToken cancellationToken = default)
    {
        var command = new WithdrawApplicationCommand
        {
            ApplicationId = applicationId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

}
