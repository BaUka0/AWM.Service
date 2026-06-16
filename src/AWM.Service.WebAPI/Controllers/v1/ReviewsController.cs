using AWM.Service.Application.Features.Workflow.Reviews.DTOs;
using AWM.Service.Application.Features.Workflow.Reviews.Queries.GetMyReviewerAssignments;
using AWM.Service.WebAPI.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing thesis work reviews.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
[ApiController]
[Authorize]
public sealed class ReviewsController : BaseController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewsController"/> class.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public ReviewsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets reviewer assignments for the currently authenticated user.
    /// </summary>
    [HttpGet("my-assignments")]
    [RequireAccess("THESIS.WORK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewerAssignmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAssignments(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyReviewerAssignmentsQuery(), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }
}
