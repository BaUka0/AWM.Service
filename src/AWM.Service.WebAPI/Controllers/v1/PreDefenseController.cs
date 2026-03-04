namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Defense.PreDefense.Commands.DistributeStudentsToCommissions;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.FinalizePreDefense;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseProtocol;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseSlots;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.RecordAttendance;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.SchedulePreDefense;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.StartReconciliation;
using AWM.Service.Application.Features.Defense.PreDefense.Commands.SubmitPreDefenseGrade;
using AWM.Service.Application.Features.Defense.PreDefense.DTOs;
using AWM.Service.Application.Features.Defense.PreDefense.Queries.GetFailedPreDefenseStudents;
using AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseAttempts;
using AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseSchedule;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using Mapster;
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
        _sender = sender;
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
    /// Get students who failed pre-defense and may need retake.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <param name="preDefenseNumber">Optional pre-defense round filter (1, 2, or 3)</param>
    /// <returns>List of failed students with attempt details</returns>
    [HttpGet("failed-students")]
    [RequireDepartmentPermission(Permission.PreDefense_View)]
    [ProducesResponseType(typeof(IReadOnlyList<FailedPreDefenseStudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFailedStudents(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        [FromQuery] int? preDefenseNumber = null)
    {
        var query = new GetFailedPreDefenseStudentsQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId,
            PreDefenseNumber = preDefenseNumber
        };

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
        var command = request.Adapt<SchedulePreDefenseCommand>() with { WorkId = workId };

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
        var command = request.Adapt<RecordAttendanceCommand>() with { AttemptId = attemptId };

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
        var command = request.Adapt<SubmitPreDefenseGradeCommand>() with { ScheduleId = scheduleId };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>
    /// Finalize a pre-defense attempt by recording the average score and pass/fail result (Secretary action).
    /// </summary>
    /// <param name="attemptId">PreDefenseAttempt ID</param>
    /// <param name="request">Finalize details including average score and pass/fail result</param>
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
        var command = request.Adapt<FinalizePreDefenseCommand>() with { AttemptId = attemptId };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Start grade reconciliation for a pre-defense schedule (Secretary action).
    /// After this, all commission members can see each other's grades.
    /// </summary>
    /// <param name="scheduleId">Schedule ID</param>
    /// <returns>No content on success</returns>
    [HttpPut("schedule/{scheduleId:long}/start-reconciliation")]
    [RequireDepartmentPermission(Permission.PreDefense_Finalize)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartReconciliation(long scheduleId)
    {
        var command = new StartReconciliationCommand { ScheduleId = scheduleId };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Distribute student works across pre-defense commissions (round-robin).
    /// </summary>
    /// <param name="request">Distribution parameters</param>
    /// <returns>Number of works distributed</returns>
    [HttpPost("distribute")]
    [RequireDepartmentPermission(Permission.PreDefense_Schedule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Distribute([FromBody] DistributeStudentsRequest request)
    {
        var command = request.Adapt<DistributeStudentsToCommissionsCommand>();

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Generate time slots for a pre-defense commission on a specific date.
    /// </summary>
    /// <param name="request">Slot generation parameters</param>
    /// <returns>Number of slots generated</returns>
    [HttpPost("generate-slots")]
    [RequireDepartmentPermission(Permission.PreDefense_Schedule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateSlots([FromBody] GeneratePreDefenseSlotsRequest request)
    {
        var command = request.Adapt<GeneratePreDefenseSlotsCommand>();

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Generate a pre-defense session protocol (ведомость) for a commission.
    /// </summary>
    /// <param name="request">Protocol generation details</param>
    /// <returns>Created protocol ID</returns>
    [HttpPost("protocols")]
    [RequireDepartmentPermission(Permission.PreDefense_Finalize)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateProtocol([FromBody] GeneratePreDefenseProtocolRequest request)
    {
        var command = request.Adapt<GeneratePreDefenseProtocolCommand>();

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
