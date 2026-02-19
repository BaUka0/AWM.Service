namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.QualityChecks.Commands.RecordCheckResult;
using AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;
using AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetChecksByWork;
using AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetPendingChecks;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
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
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get all quality checks for a specific work.
    /// </summary>
    /// <param name="workId">StudentWork ID</param>
    /// <returns>List of quality check records ordered by type and attempt</returns>
    [HttpGet("by-work/{workId:long}")]
    [RequireDepartmentPermission(Permission.QualityChecks_View)]
    [ProducesResponseType(typeof(IReadOnlyList<QualityCheckResponse>), StatusCodes.Status200OK)]
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

        var response = result.Value
            .Select(c => new QualityCheckResponse
            {
                Id = c.Id,
                WorkId = c.WorkId,
                CheckType = c.CheckType,
                AttemptNumber = c.AttemptNumber,
                IsPassed = c.IsPassed,
                ResultValue = c.ResultValue,
                Comment = c.Comment,
                DocumentPath = c.DocumentPath,
                AssignedExpertId = c.AssignedExpertId,
                CheckedAt = c.CheckedAt
            })
            .ToList();

        return Ok(response);
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
    [ProducesResponseType(typeof(IReadOnlyList<QualityCheckResponse>), StatusCodes.Status200OK)]
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

        var response = result.Value
            .Select(c => new QualityCheckResponse
            {
                Id = c.Id,
                WorkId = c.WorkId,
                CheckType = c.CheckType,
                AttemptNumber = c.AttemptNumber,
                IsPassed = c.IsPassed,
                ResultValue = c.ResultValue,
                Comment = c.Comment,
                DocumentPath = c.DocumentPath,
                AssignedExpertId = c.AssignedExpertId,
                CheckedAt = c.CheckedAt
            })
            .ToList();

        return Ok(response);
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
        var command = new SubmitForCheckCommand
        {
            WorkId = workId,
            CheckType = request.CheckType,
            Comment = request.Comment
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetByWork), new { workId }, result.Value);
    }

    /// <summary>
    /// Record a quality check result (expert action).
    /// </summary>
    /// <param name="workId">StudentWork ID</param>
    /// <param name="request">Check result details</param>
    /// <returns>Created quality check ID</returns>
    [HttpPost("works/{workId:long}/record")]
    [RequireDepartmentPermission(Permission.QualityChecks_Perform)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RecordResult(long workId, [FromBody] RecordCheckResultRequest request)
    {
        var command = new RecordCheckResultCommand
        {
            WorkId = workId,
            CheckType = request.CheckType,
            IsPassed = request.IsPassed,
            ResultValue = request.ResultValue,
            Comment = request.Comment,
            DocumentPath = request.DocumentPath
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetByWork), new { workId }, result.Value);
    }
}
