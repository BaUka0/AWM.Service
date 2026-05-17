namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Workflow.Queries.GetAllWorkTypes;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses.Workflow;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for WorkType dictionary operations.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class WorkTypesController : BaseController
{
    private readonly ISender _sender;

    public WorkTypesController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// Get all available work types.
    /// Used by the frontend to dynamically map work type IDs to their names.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all work types</returns>
    [HttpGet]
    [RequireAccess("Topics", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<WorkTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetAllWorkTypesQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<WorkTypeResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Create a new work type.
    /// </summary>
    /// <param name="request">Work type creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created work type ID.</returns>
    [HttpPost]
    [RequireAccess("Org_Departments", "Create")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateWorkType(
        [FromBody] AWM.Service.WebAPI.Common.Contracts.Requests.Workflow.CreateWorkTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<AWM.Service.Application.Features.Workflow.Commands.CreateWorkType.CreateWorkTypeCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(
            nameof(GetAll),
            new { version = "1.0" },
            result.Value);
    }

    /// <summary>
    /// Update an existing work type.
    /// </summary>
    /// <param name="id">Work type ID.</param>
    /// <param name="request">Updated work type data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [RequireAccess("Org_Departments", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateWorkType(
        [FromRoute] int id,
        [FromBody] AWM.Service.WebAPI.Common.Contracts.Requests.Workflow.UpdateWorkTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<AWM.Service.Application.Features.Workflow.Commands.UpdateWorkType.UpdateWorkTypeCommand>() with { Id = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete a work type.
    /// </summary>
    /// <param name="id">Work type ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [RequireAccess("Org_Departments", "Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteWorkType(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var command = new AWM.Service.Application.Features.Workflow.Commands.DeleteWorkType.DeleteWorkTypeCommand { Id = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
