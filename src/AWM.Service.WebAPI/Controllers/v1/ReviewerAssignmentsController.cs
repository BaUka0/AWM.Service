namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Reviews.Queries.GetMyReviewerAssignments;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for reviewer assignment lookups ("My Reviews").
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviewer-assignments")]
[ApiController]
[Produces("application/json")]
public sealed class ReviewerAssignmentsController : BaseController
{
    private readonly ISender _sender;

    public ReviewerAssignmentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get assignments for the current reviewer ("My Reviews").
    /// </summary>
    [HttpGet("my")]
    [RequireAccess("Reviews", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewerAssignmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyAssignments(CancellationToken cancellationToken = default)
    {
        var query = new GetMyReviewerAssignmentsQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<IReadOnlyList<ReviewerAssignmentResponse>>());
    }
}
