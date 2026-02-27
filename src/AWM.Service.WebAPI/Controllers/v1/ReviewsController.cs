namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Reviews.Commands.CreateSupervisorReview;
using AWM.Service.Application.Features.Thesis.Reviews.Commands.UploadReview;
using AWM.Service.Application.Features.Thesis.Reviews.Queries.GetReviewsByWork;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing supervisor and external reviews of student works.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/works/{workId:long}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class ReviewsController : BaseController
{
    private readonly ISender _sender;

    public ReviewsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all reviews (supervisor and external) for a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A summary object containing the supervisor review and external reviews.</returns>
    [HttpGet]
    [RequireDepartmentPermission(Permission.Reviews_View)]
    [ProducesResponseType(typeof(WorkReviewsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviewsByWork(long workId, CancellationToken cancellationToken = default)
    {
        var query = new GetReviewsByWorkQuery { WorkId = workId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var dto = result.Value;

        SupervisorReviewResponse? supervisorResponse = null;
        if (dto.SupervisorReview is not null)
        {
            supervisorResponse = dto.SupervisorReview.Adapt<SupervisorReviewResponse>();
        }

        var reviewResponses = dto.Reviews.Adapt<List<ReviewResponse>>();

        var response = new WorkReviewsResponse
        {
            SupervisorReview = supervisorResponse,
            Reviews = reviewResponses
        };

        return Ok(response);
    }

    /// <summary>
    /// Create or update a supervisor review for a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="request">Upload request containing review text and optional file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of the created or updated supervisor review.</returns>
    [HttpPost("supervisor")]
    [RequireDepartmentPermission(Permission.Reviews_CreateSupervisor)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateOrUpdateSupervisorReview(
        long workId,
        [FromForm] CreateSupervisorReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateSupervisorReviewCommand>() with { WorkId = workId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        // Returning 200 OK with ID instead of 201 Created because this endpoint acts as an upsert
        return Ok(result.Value);
    }

    /// <summary>
    /// Upload or update an external review for a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="reviewId">Review ID.</param>
    /// <param name="request">Upload request containing review text and/or file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("external/{reviewId:long}")]
    [RequireDepartmentPermission(Permission.Reviews_UploadExternal)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadExternalReview(
        long workId, // workId is maintained for routing semantics
        long reviewId,
        [FromForm] UploadReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UploadReviewCommand>() with { WorkId = workId, ReviewId = reviewId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
