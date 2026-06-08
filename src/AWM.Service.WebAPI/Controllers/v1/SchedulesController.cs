using AWM.Service.Application.Features.Defense.Schedules.Commands.AddGrade;
using AWM.Service.Application.Features.Defense.Schedules.Commands.DeleteSchedule;
using AWM.Service.Application.Features.Defense.Schedules.Commands.GenerateSchedule;
using AWM.Service.Application.Features.Defense.Schedules.Commands.StartReconciliation;
using AWM.Service.Application.Features.Defense.Schedules.Commands.UpdateSchedule;
using AWM.Service.Application.Features.Defense.Schedules.Queries.GetMyDefenseStep;
using AWM.Service.Application.Features.Defense.Schedules.Queries.GetScheduleByWork;
using AWM.Service.Application.Features.Defense.Schedules.Queries.GetScheduleGrades;
using AWM.Service.Application.Features.Defense.Schedules.Queries.GetSchedulesByCommission;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using AWM.Service.WebAPI.Common.Contracts.Responses.Defense;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
    [RequireAccess("DEFENSE.SCHEDULE", "Read")]
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
    [RequireAccess("DEFENSE.SCHEDULE", "Update")]
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
    /// Updates a defense schedule slot (reschedule or change commission).
    /// </summary>
    [HttpPut("{id}")]
    [RequireAccess("DEFENSE.SCHEDULE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSchedule(
        long id,
        [FromBody] UpdateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateScheduleCommand(id, request.CommissionId, request.DefenseDate, request.Location);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Deletes a defense schedule slot (unschedules the student).
    /// </summary>
    [HttpDelete("{id}")]
    [RequireAccess("DEFENSE.SCHEDULE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSchedule(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteScheduleCommand(id);
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
    [RequireAccess("DEFENSE.SCHEDULE", "Read")]
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
    [RequireAccess("DEFENSE.GRADE", "Create")]
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
    [RequireAccess("DEFENSE.SCHEDULE", "Update")]
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

    /// <summary>
    /// Gets defense schedule slots for a commission.
    /// </summary>
    [HttpGet]
    [RequireAccess("DEFENSE.SCHEDULE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<CommissionScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedulesByCommission(
        [FromQuery] int commissionId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSchedulesByCommissionQuery(commissionId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Gets the defense schedule for a specific student work.
    /// </summary>
    [HttpGet("by-work/{workId:long}")]
    [RequireAccess("DEFENSE.SCHEDULE", "Read")]
    [ProducesResponseType(typeof(ScheduleByWorkResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByWorkId(
        long workId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetScheduleByWorkQuery(workId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }
}
