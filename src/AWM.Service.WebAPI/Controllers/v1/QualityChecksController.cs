using AWM.Service.Application.Features.Workflow.Checks.Commands.CompleteQualityCheck;
using AWM.Service.Application.Features.Workflow.Checks.Commands.DeleteCheckConfiguration;
using AWM.Service.Application.Features.Workflow.Checks.Commands.SaveCheckConfiguration;
using AWM.Service.Application.Features.Workflow.Checks.Commands.SaveExpertAssignments;
using AWM.Service.Application.Features.Workflow.Checks.Commands.SubmitForCheck;
using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using AWM.Service.Application.Features.Workflow.Checks.Queries.GetAssignedExperts;
using AWM.Service.Application.Features.Workflow.Checks.Queries.GetActiveCheckConfigurations;
using AWM.Service.Application.Features.Workflow.Checks.Queries.GetCheckConfigurations;
using AWM.Service.Application.Features.Workflow.Checks.Queries.GetPendingChecks;
using AWM.Service.Application.Features.Workflow.Checks.Queries.GetQualityChecksByWork;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Checks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing quality checks, configurations, and expert assignments.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/quality-checks")]
[ApiController]
[Authorize]
public sealed class QualityChecksController : BaseController
{
    private readonly ISender _sender;

    public QualityChecksController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all quality checks for a specific student work.
    /// </summary>
    [HttpGet("by-work/{workId:long}")]
    [RequireAccess("THESIS.CHECK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<QualityCheckDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQualityChecksByWork(long workId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetQualityChecksByWorkQuery(workId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Submits a work for checking (creates a new quality check attempt).
    /// </summary>
    [HttpPost("works/{workId:long}/submit")]
    [RequireAccess("THESIS.WORK", "Update")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SubmitForCheck(
        long workId,
        [FromBody] SubmitForCheckRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new SubmitForCheckCommand(workId, request.CheckTypeId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Gets pending quality checks for experts.
    /// </summary>
    [HttpGet("pending")]
    [RequireAccess("THESIS.CHECK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<QualityCheckDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPendingChecks(
        [FromQuery] int orgUnitId,
        [FromQuery] int semesterId,
        [FromQuery] int? checkTypeId,
        [FromQuery] bool includeCompleted = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetPendingChecksQuery(orgUnitId, semesterId, checkTypeId, includeCompleted), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Records expert decision on a quality check.
    /// </summary>
    [HttpPost("works/{workId:long}/{checkId:long}/complete")]
    [RequireAccess("THESIS.CHECK", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CompleteQualityCheck(
        long workId,
        long checkId,
        [FromBody] CompleteQualityCheckRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteQualityCheckCommand(
            workId,
            checkId,
            request.IsPassed,
            request.ResultValue,
            request.Comment,
            request.AttachmentId);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok();
    }

    /// <summary>
    /// Gets check configurations for a department.
    /// </summary>
    [HttpGet("configurations")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<CheckConfigurationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCheckConfigurations([FromQuery] int orgUnitId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCheckConfigurationsQuery(orgUnitId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Gets active check configurations for a student's org unit and speciality.
    /// Accessible to students and experts (THESIS.CHECK Read).
    /// </summary>
    [HttpGet("configurations/active")]
    [RequireAccess("THESIS.CHECK", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<CheckConfigurationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetActiveCheckConfigurations(
        [FromQuery] int orgUnitId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetActiveCheckConfigurationsQuery(orgUnitId, specialityId), cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Saves or updates a check configuration for a department.
    /// </summary>
    [HttpPost("configurations")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SaveCheckConfiguration(
        [FromBody] SaveCheckConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<SaveCheckConfigurationCommand>();
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a check configuration.
    /// </summary>
    [HttpDelete("configurations/{id:int}")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCheckConfiguration(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCheckConfigurationCommand(id), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok();
    }

    /// <summary>
    /// Gets assigned experts for a department.
    /// </summary>
    [HttpGet("experts")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<ExpertAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAssignedExperts([FromQuery] int orgUnitId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAssignedExpertsQuery(orgUnitId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Saves expert assignments for a department.
    /// </summary>
    [HttpPost("experts")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SaveExpertAssignments(
        [FromBody] SaveExpertAssignmentsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SaveExpertAssignmentsCommand(request.OrgUnitId, request.Assignments);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }
        return Ok();
    }
}
