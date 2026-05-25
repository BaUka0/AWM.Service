using AWM.Service.Application.Features.Workflow.Supervisors.Commands.ApproveSupervisors;
using AWM.Service.Application.Features.Workflow.Supervisors.Commands.RemoveSupervisor;
using AWM.Service.Application.Features.Workflow.Supervisors.Commands.UpdateSupervisorWorkload;
using AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetApprovedSupervisors;
using AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetOrgUnitTeachers;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests;
using AWM.Service.WebAPI.Common.Contracts.Responses;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing organization unit supervisors and their workloads.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/org-units/{orgUnitId}/supervisors")]
[ApiController]
[Authorize]
public sealed class SupervisorsController : BaseController
{
    private readonly ISender _sender;

    public SupervisorsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all organization unit teachers available to be appointed as supervisors.
    /// </summary>
    /// <param name="orgUnitId">The ID of the organization unit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of available teachers.</returns>
    [HttpGet("available")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableTeachers(int orgUnitId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrgUnitTeachersQuery(orgUnitId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TeacherResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Gets currently approved supervisors for an organization unit/semester/speciality.
    /// </summary>
    /// <param name="orgUnitId">The ID of the organization unit.</param>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="specialityId">Optional ID of the speciality.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of approved supervisors with their workloads.</returns>
    [HttpGet("approved")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApprovedSupervisors(
        int orgUnitId, 
        [FromQuery] int semesterId, 
        [FromQuery] int? specialityId, 
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApprovedSupervisorsQuery(orgUnitId, semesterId, specialityId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TeacherResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Approves or updates the list of supervisors for an organization unit/semester/speciality.
    /// </summary>
    /// <param name="orgUnitId">The ID of the organization unit.</param>
    /// <param name="request">The approval request containing supervisors and workloads.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("approve")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveSupervisors(
        int orgUnitId, 
        [FromBody] ApproveSupervisorsRequest request, 
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<ApproveSupervisorsCommand>() with { OrgUnitId = orgUnitId };

        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Updates the workload limit for a specific supervisor in a specific period.
    /// </summary>
    /// <param name="orgUnitId">The ID of the organization unit.</param>
    /// <param name="userId">The ID of the user (teacher).</param>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="specialityId">Optional ID of the speciality.</param>
    /// <param name="request">The update request containing the new workload limit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPut("{userId}/workload")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkload(
        int orgUnitId,
        int userId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        [FromBody] UpdateSupervisorWorkloadRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSupervisorWorkloadCommand(
            orgUnitId,
            userId,
            semesterId,
            specialityId,
            request.MaxWorkload);

        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Removes a supervisor from the approved list for the specified period.
    /// </summary>
    /// <param name="orgUnitId">The ID of the organization unit.</param>
    /// <param name="userId">The ID of the user (teacher).</param>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="specialityId">Optional ID of the speciality.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpDelete("{userId}")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSupervisor(
        int orgUnitId,
        int userId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveSupervisorCommand(
            orgUnitId,
            userId,
            semesterId,
            specialityId);

        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }
}
