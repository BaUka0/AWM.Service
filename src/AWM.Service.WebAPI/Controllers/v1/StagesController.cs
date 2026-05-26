using AWM.Service.Application.Features.Workflow.Stages.Commands.SetStagesPeriods;
using AWM.Service.Application.Features.Workflow.Stages.Queries.GetStagesPeriods;
using AWM.Service.Application.Features.Workflow.Stages.Queries.GetOrgUnitSpecialities;
using AWM.Service.Application.Features.Workflow.Stages.Commands.ResetStages;
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

    /// <summary>
    /// Gets all unique specialities associated with an OrgUnit (department).
    /// If OrgUnitId is not provided, it will be derived from the current user's main position in University SoT.
    /// </summary>
    /// <param name="orgUnitId">Optional department ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of specialities.</returns>
    [HttpGet("specialities")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSpecialities(
        [FromQuery] int? orgUnitId,
        CancellationToken cancellationToken)
    {
        var query = new GetOrgUnitSpecialitiesQuery(orgUnitId);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<SpecialityResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Resets/deletes custom stages override for a specific speciality, falling back to department-wide stages.
    /// If OrgUnitId is not provided, it will be derived from the current user's main position in University SoT.
    /// </summary>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="specialityId">The ID of the speciality to reset.</param>
    /// <param name="orgUnitId">Optional department ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpDelete("override")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetOverride(
        [FromQuery] int semesterId,
        [FromQuery] int specialityId,
        [FromQuery] int? orgUnitId,
        CancellationToken cancellationToken)
    {
        var command = new ResetStagesCommand(semesterId, specialityId, orgUnitId);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }
}
