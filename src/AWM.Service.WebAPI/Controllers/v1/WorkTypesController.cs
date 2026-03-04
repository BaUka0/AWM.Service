namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Workflow.Queries.GetAllWorkTypes;
using AWM.Service.Domain.Auth.Enums;
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
    [RequirePermission(Permission.Topics_View)]
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
}
