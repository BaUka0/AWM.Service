namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Defense.PreDefense.Commands.FinalizePreDefense;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.RecordAttendance;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.SchedulePreDefense;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.SubmitPreDefenseGrade;
using AWM.Service.Application.Features.Defense.PreDefense.DTOs;
using AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseAttempts;
using AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseSchedule;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Pre-defense scheduling, attendance, grading, and finalization.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/pre-defense")]
[Produces("application/json")]
public class PreDefenseController : BaseController
{
    private readonly ISender _sender;

    public PreDefenseController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get the schedule (all slots) for a pre-defense commission.
    /// </summary>
    /// <param name="commissionId">Commission ID</param>
    /// <returns>List of schedule slots ordered by defense date</returns>
    [HttpGet("schedule")]
    [RequireDepartmentPermission(Permission.PreDefense_View)]
    [ProducesResponseType(typeof(IReadOnlyList<PreDefenseScheduleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSchedule([FromQuery] int commissionId)
    {
        var query = new GetPreDefenseScheduleQuery { CommissionId = commissionId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get all pre-defense attempts for a student work.
    /// </summary>
    /// <param name="workId">StudentWork ID</param>
    /// <returns>List of attempts ordered by attempt number</returns>
    [HttpGet("works/{workId:long}/attempts")]
    [RequireDepartmentPermission(Permission.PreDefense_View)]
    [ProducesResponseType(typeof(IReadOnlyList<PreDefenseAttemptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAttempts(long workId)
    {
        var query = new GetPreDefenseAttemptsQuery { WorkId = workId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Schedule a student work for a pre-defense slot (Secretary action).
    /// Creates a schedule slot and automatically creates a PreDefenseAttempt.
    /// </summary>
    /// <param name="workId">StudentWork ID to schedule</param>
    /// <param name="request">Schedule details</param>
    /// <returns>Created schedule ID</returns>
    [HttpPost("works/{workId:long}/schedule")]
    [RequireDepartmentPermission(Permission.PreDefense_Schedule)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Schedule(long workId, [FromBody] SchedulePreDefenseRequest request)
    {
        var command = new SchedulePreDefenseCommand
        {
            CommissionId = request.CommissionId,
            WorkId = workId,
            DefenseDate = request.DefenseDate,
            Location = request.Location
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetAttempts), new { workId }, result.Value);
    }

    /// <summary>
    /// Record attendance for a pre-defense attempt (Secretary action).
    /// </summary>
    /// <param name="attemptId">PreDefenseAttempt ID</param>
    /// <param name="request">Attendance details</param>
    /// <returns>No content on success</returns>
    [HttpPut("attempts/{attemptId:long}/attendance")]
    [RequireDepartmentPermission(Permission.PreDefense_RecordAttendance)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RecordAttendance(long attemptId, [FromBody] RecordAttendanceRequest request)
    {
        var command = new RecordAttendanceCommand
        {
            AttemptId = attemptId,
            AttendanceStatus = request.AttendanceStatus,
            IsExcused = request.IsExcused
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Submit a grade for a scheduled pre-defense slot (Commission member action).
    /// </summary>
    /// <param name="scheduleId">Schedule ID</param>
    /// <param name="request">Grade details</param>
    /// <returns>Created grade ID</returns>
    [HttpPost("schedule/{scheduleId:long}/grades")]
    [RequireDepartmentPermission(Permission.PreDefense_Grade)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitGrade(long scheduleId, [FromBody] SubmitPreDefenseGradeRequest request)
    {
        var command = new SubmitPreDefenseGradeCommand
        {
            ScheduleId = scheduleId,
            MemberId = request.MemberId,
            CriteriaId = request.CriteriaId,
            Score = request.Score,
            Comment = request.Comment
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>
    /// Finalize a pre-defense attempt by recording the average score and pass/fail result (Secretary action).
    /// </summary>
    /// <param name="attemptId">PreDefenseAttempt ID</param>
    /// <param name="averageScore">Final average score</param>
    /// <param name="isPassed">Whether the student passed</param>
    /// <returns>No content on success</returns>
    [HttpPut("attempts/{attemptId:long}/finalize")]
    [RequireDepartmentPermission(Permission.PreDefense_Finalize)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Finalize(long attemptId, [FromBody] FinalizePreDefenseRequest request)
    {
        var command = new FinalizePreDefenseCommand
        {
            AttemptId = attemptId,
            AverageScore = request.AverageScore,
            IsPassed = request.IsPassed
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
