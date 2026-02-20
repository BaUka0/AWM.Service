namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Defense.Evaluation.Commands.FinalizeDefense;
using AWM.Service.Application.Features.Defense.Evaluation.Commands.SubmitGrade;
using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using AWM.Service.Application.Features.Defense.Evaluation.Queries.GetEvaluationCriteria;
using AWM.Service.Application.Features.Defense.Evaluation.Queries.GetGradesByWork;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for defense evaluation — grading, criteria, and finalization.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/evaluation")]
[Produces("application/json")]
public class EvaluationController : BaseController
{
    private readonly ISender _sender;

    public EvaluationController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get evaluation criteria by work type with optional department filtering.
    /// </summary>
    /// <param name="workTypeId">Work type ID</param>
    /// <param name="departmentId">Optional department ID</param>
    /// <returns>List of evaluation criteria</returns>
    [HttpGet("criteria")]
    [RequireDepartmentPermission(Permission.Defense_View)]
    [ProducesResponseType(typeof(IReadOnlyList<EvaluationCriteriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCriteria([FromQuery] int workTypeId, [FromQuery] int? departmentId = null)
    {
        var query = new GetEvaluationCriteriaQuery
        {
            WorkTypeId = workTypeId,
            DepartmentId = departmentId
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get all grades for a specific defense schedule (slot).
    /// </summary>
    /// <param name="scheduleId">Schedule ID</param>
    /// <returns>List of grades</returns>
    [HttpGet("schedule/{scheduleId:long}/grades")]
    [RequireDepartmentPermission(Permission.Defense_View)]
    [ProducesResponseType(typeof(IReadOnlyList<GradeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGrades(long scheduleId)
    {
        var query = new GetGradesByWorkQuery { ScheduleId = scheduleId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Submit a grade for a defense schedule slot (GAK member action).
    /// </summary>
    /// <param name="scheduleId">Schedule ID</param>
    /// <param name="request">Grade details</param>
    /// <returns>Created grade ID</returns>
    [HttpPost("schedule/{scheduleId:long}/grades")]
    [RequireDepartmentPermission(Permission.Defense_Grade)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitGrade(long scheduleId, [FromBody] SubmitGradeRequest request)
    {
        var command = new SubmitGradeCommand
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
    /// Finalize a defense session (Secretary action).
    /// Locks the associated protocol — no more changes allowed.
    /// </summary>
    /// <param name="scheduleId">Schedule ID</param>
    /// <returns>No content on success</returns>
    [HttpPut("schedule/{scheduleId:long}/finalize")]
    [RequireDepartmentPermission(Permission.Defense_Finalize)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FinalizeDefense(long scheduleId)
    {
        var command = new FinalizeDefenseCommand { ScheduleId = scheduleId };
        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
