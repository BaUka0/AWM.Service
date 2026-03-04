namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.QualityChecks.Commands.AssignExperts;
using AWM.Service.Application.Features.Thesis.QualityChecks.Commands.RecordCheckResult;
using AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;
using AWM.Service.Application.Features.Thesis.QualityChecks.DTOs;
using AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetChecksByWork;
using AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetPendingChecks;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Quality Checks (NormControl, SoftwareCheck, AntiPlagiarism).
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/quality-checks")]
[Produces("application/json")]
public class QualityChecksController : BaseController
{
    private readonly ISender _sender;

    public QualityChecksController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all quality checks for a specific work.
    /// </summary>
    /// <param name="workId">StudentWork ID</param>
    /// <returns>List of quality check records ordered by type and attempt</returns>
    [HttpGet("by-work/{workId:long}")]
    [RequireDepartmentPermission(Permission.QualityChecks_View)]
    [ProducesResponseType(typeof(IReadOnlyList<QualityCheckDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByWork(long workId)
    {
        var query = new GetChecksByWorkQuery { WorkId = workId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get all pending quality checks for a department (expert's work queue).
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <param name="checkType">Optional check type filter</param>
    /// <returns>List of pending quality checks</returns>
    [HttpGet("pending")]
    [RequireDepartmentPermission(Permission.QualityChecks_Perform)]
    [ProducesResponseType(typeof(IReadOnlyList<QualityCheckDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPending(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        [FromQuery] CheckType? checkType = null)
    {
        var query = new GetPendingChecksQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId,
            CheckType = checkType
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Submit a work for a quality check (student action).
    /// Creates a pending check record that an expert will review.
    /// </summary>
    /// <param name="workId">StudentWork ID</param>
    /// <param name="request">Submit request with check type</param>
    /// <returns>Created quality check ID</returns>
    [HttpPost("works/{workId:long}/submit")]
    [RequireDepartmentPermission(Permission.QualityChecks_Submit)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Submit(long workId, [FromBody] SubmitForCheckRequest request)
    {
        var command = request.Adapt<SubmitForCheckCommand>() with { WorkId = workId };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetByWork), new { workId }, result.Value);
    }

    /// <summary>
    /// Record a quality check result (expert action).
    /// Updates the existing pending check record created by the student's submission.
    /// </summary>
    /// <param name="workId">StudentWork ID</param>
    /// <param name="checkId">ID of the pending QualityCheck to complete (returned by SubmitForCheck)</param>
    /// <param name="request">Check result details</param>
    /// <returns>Updated quality check ID</returns>
    [HttpPut("works/{workId:long}/checks/{checkId:long}/record")]
    [RequireDepartmentPermission(Permission.QualityChecks_Perform)]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RecordResult(long workId, long checkId, [FromBody] RecordCheckResultRequest request)
    {
        var command = request.Adapt<RecordCheckResultCommand>() with { WorkId = workId, CheckId = checkId };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Assign experts to quality check types for a department.
    /// Creates Expert entities linking users to specific expertise types.
    /// </summary>
    /// <param name="request">Expert assignment details</param>
    /// <returns>Number of new experts created</returns>
    [HttpPost("assign-experts")]
    [RequireDepartmentPermission(Permission.Experts_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignExperts([FromBody] AssignExpertsRequest request)
    {
        var assignments = request.Assignments
            .Select(a => new ExpertAssignmentDto(a.UserId, a.ExpertiseType))
            .ToList();

        var command = new AssignExpertsCommand
        {
            DepartmentId = request.DepartmentId,
            Assignments = assignments
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }
}
