using AWM.Service.Application.Features.Workflow.Stages.Commands.SetStagesPeriods;
using AWM.Service.Application.Features.Workflow.Stages.Queries.GetStagesPeriods;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests;
using AWM.Service.WebAPI.Common.Contracts.Responses;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing workflow stages and periods.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stages")]
[ApiController]
[Authorize]
public sealed class StagesController : BaseController
{
    private readonly ISender _sender;

    public StagesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets the periods for workflow stages.
    /// If OrgUnitId is not provided, it will be derived from the current user's main position in University SoT.
    /// </summary>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="orgUnitId">Optional ID of the department.</param>
    /// <param name="specialityId">Optional ID of the speciality.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of stage periods.</returns>
    [HttpGet("periods")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<StagePeriodResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPeriods(
        [FromQuery] int semesterId,
        [FromQuery] int? orgUnitId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var query = new GetStagesPeriodsQuery(semesterId, orgUnitId, specialityId);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<StagePeriodResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Sets or updates the periods for multiple workflow stages.
    /// If OrgUnitId is not provided, it will be derived from the current user's main position in University SoT.
    /// </summary>
    /// <param name="request">The request containing semester and periods.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("periods")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetPeriods(
        [FromBody] SetStagesPeriodsRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<SetStagesPeriodsCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }
}
