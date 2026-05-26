using AWM.Service.Application.Features.Workflow.Reviewers.Commands.CreateReviewer;
using AWM.Service.Application.Features.Workflow.Reviewers.Commands.DeleteReviewer;
using AWM.Service.Application.Features.Workflow.Reviewers.Commands.UpdateReviewer;
using AWM.Service.Application.Features.Workflow.Reviewers.DTOs;
using AWM.Service.Application.Features.Workflow.Reviewers.Queries.GetReviewers;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Reviewers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing external reviewers database.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviewers")]
[ApiController]
[Authorize]
public sealed class ReviewersController : BaseController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewersController"/> class.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public ReviewersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets the list of active reviewers, optionally filtered by search term.
    /// </summary>
    [HttpGet]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewers([FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetReviewersQuery(searchTerm), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new external reviewer.
    /// </summary>
    [HttpPost]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReviewer([FromBody] CreateReviewerRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateReviewerCommand(
            request.FullName,
            request.Position,
            request.AcademicDegree,
            request.Organization,
            request.Email,
            request.Phone);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Updates an existing external reviewer.
    /// </summary>
    [HttpPut("{id:int}")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReviewer(int id, [FromBody] UpdateReviewerRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateReviewerCommand(
            id,
            request.FullName,
            request.Position,
            request.AcademicDegree,
            request.Organization,
            request.Email,
            request.Phone);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok();
    }

    /// <summary>
    /// Deletes (soft-deletes) an external reviewer.
    /// </summary>
    [HttpDelete("{id:int}")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReviewer(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteReviewerCommand(id), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok();
    }
}
