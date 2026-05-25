using AWM.Service.Application.Features.Defense.Schedules.Commands.AddGrade;
using AWM.Service.Application.Features.Defense.Schedules.Commands.GenerateSchedule;
using AWM.Service.Application.Features.Defense.Schedules.Commands.StartReconciliation;
using AWM.Service.Application.Features.Defense.Schedules.Queries.GetMyDefenseStep;
using AWM.Service.Application.Features.Defense.Schedules.Queries.GetScheduleGrades;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing defense schedules, grading, and reconciliation.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/schedules")]
[ApiController]
[Authorize]
public sealed class SchedulesController : BaseController
{
    private readonly ISender _sender;

    public SchedulesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets the current defense step information for the authenticated student.
    /// </summary>
    [HttpGet("my-defense-step")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDefenseStep(CancellationToken cancellationToken)
    {
        var query = new GetMyDefenseStepQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Generates defense schedule slots for a commission.
    /// </summary>
    [HttpPost("generate")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateSchedule(
        [FromBody] GenerateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<GenerateScheduleCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Adds a grade from a commission member for a specific scheduled defense.
    /// </summary>
    [HttpGet("{id}/grades")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    public async Task<IActionResult> GetGrades(
        long id,
        CancellationToken cancellationToken)
    {
        var query = new GetScheduleGradesQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Adds a grade from a commission member for a specific scheduled defense.
    /// </summary>
    [HttpPost("{id}/grades")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> AddGrade(
        long id,
        [FromBody] AddGradeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddGradeCommand(id, request.CriteriaId, request.Score, request.Comment);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Starts the grade reconciliation phase for a specific scheduled defense.
    /// Only accessible by commission chairman or secretary.
    /// </summary>
    [HttpPost("{id}/start-reconciliation")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> StartReconciliation(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new StartReconciliationCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }
}
