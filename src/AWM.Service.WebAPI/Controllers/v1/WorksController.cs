using AWM.Service.Application.Features.Workflow.Works.Commands.AssignReviewer;
using AWM.Service.Application.Features.Workflow.Works.Commands.SubmitSupervisorReview;
using AWM.Service.Application.Features.Workflow.Reviews.Commands.UploadRecension;
using AWM.Service.Application.Features.Workflow.Works.Queries.GetMySupervisedWorks;
using AWM.Service.Application.Features.Workflow.Works.Queries.GetMyWorkProgress;
using AWM.Service.Application.Features.Workflow.Reviews.Queries.GetReviewsByWork;
using AWM.Service.Application.Features.Workflow.Reviews.Queries.GetReviewStatus;
using AWM.Service.Application.Features.Workflow.Reviews.DTOs;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Works;
using AWM.Service.Application.Features.Workflow.Works.Queries.GetDefenseReadiness;
using AWM.Service.Application.Features.Workflow.Works.Commands.AdmitToDefense;
using AWM.Service.Application.Features.Workflow.Works.DTOs;
using AWM.Service.WebAPI.Common.Contracts.Responses.Works;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing student thesis work progress, supervisor reviews, and reviewer assignments.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/works")]
[ApiController]
[Authorize]
public sealed class WorksController : BaseController
{
    private readonly ISender _sender;

    public WorksController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets the current student's thesis work progress.
    /// </summary>
    [HttpGet("my-progress")]
    [RequireAccess("THESIS.WORK", "Read")]
    [ProducesResponseType(typeof(WorkProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProgress(CancellationToken ct)
    {
        var result = await _sender.Send(new GetMyWorkProgressQuery(), ct);
        
        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<WorkProgressResponse>());
    }

    /// <summary>
    /// Gets works supervised by the current teacher.
    /// </summary>
    [HttpGet("my-supervised")]
    [RequireAccess("THESIS.WORK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<SupervisedWorkResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySupervised(CancellationToken ct)
    {
        var result = await _sender.Send(new GetMySupervisedWorksQuery(), ct);
        
        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<IReadOnlyList<SupervisedWorkResponse>>());
    }

    /// <summary>
    /// Uploads the scientific supervisor's feedback and review.
    /// </summary>
    [HttpPost("{workId:long}/reviews/supervisor")]
    [RequireAccess("THESIS.REVIEW", "Create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitSupervisorReview(
        long workId, 
        [FromForm] SubmitSupervisorReviewRequest request, 
        CancellationToken ct)
    {
        var command = new SubmitSupervisorReviewCommand(
            workId,
            request.File.OpenReadStream(),
            request.File.FileName,
            request.File.Length,
            request.File.ContentType,
            request.Comment ?? string.Empty);

        var result = await _sender.Send(command, ct);
        
        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }

    /// <summary>
    /// Assigns an external reviewer to a student work.
    /// </summary>
    [HttpPost("{workId:long}/assign-reviewer")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignReviewer(
        long workId, 
        [FromBody] AssignReviewerRequest request, 
        CancellationToken ct)
    {
        var command = new AssignReviewerCommand(workId, request.ReviewerId);
        var result = await _sender.Send(command, ct);
        
        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }

    /// <summary>
    /// Uploads the external reviewer's feedback and review (recension).
    /// </summary>
    [HttpPost("{workId:long}/reviews/external")]
    [RequireAccess("THESIS.REVIEW", "Create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitExternalReview(
        long workId,
        [FromForm] SubmitExternalReviewRequest request,
        CancellationToken ct)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        using var stream = request.File.OpenReadStream();
        var command = new UploadRecensionCommand(
            workId,
            null, // Auto-resolved by handler
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            stream
        );

        var result = await _sender.Send(command, ct);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Gets all reviews for a specific student work.
    /// </summary>
    [HttpGet("{workId:long}/reviews")]
    [RequireAccess("THESIS.WORK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<WorkReviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewsByWork(long workId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetReviewsByWorkQuery(workId), ct);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Gets the review status of works in a department.
    /// </summary>
    [HttpGet("review-status")]
    [RequireAccess("THESIS.WORK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<WorkReviewStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewStatus(
        [FromQuery] int orgUnitId,
        [FromQuery] int semesterId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetReviewStatusQuery(orgUnitId, semesterId), ct);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Gets defense readiness statuses for the department.
    /// </summary>
    [HttpGet("defense-readiness")]
    [RequireAccess("THESIS.WORK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DefenseReadinessDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDefenseReadiness(
        [FromQuery] int orgUnitId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetDefenseReadinessQuery(orgUnitId, semesterId, specialityId), ct);
        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Admits a student work to final defense (GAK).
    /// </summary>
    [HttpPost("{workId:long}/admit")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AdmitToDefense(long workId, CancellationToken ct)
    {
        var result = await _sender.Send(new AdmitToDefenseCommand(workId), ct);
        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }
}
