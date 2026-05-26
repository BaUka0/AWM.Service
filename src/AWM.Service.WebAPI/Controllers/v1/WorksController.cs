using AWM.Service.Application.Features.Workflow.Works.Commands.AssignReviewer;
using AWM.Service.Application.Features.Workflow.Works.Commands.SubmitSupervisorReview;
using AWM.Service.Application.Features.Workflow.Works.Queries.GetMySupervisedWorks;
using AWM.Service.Application.Features.Workflow.Works.Queries.GetMyWorkProgress;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Works;
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
}
