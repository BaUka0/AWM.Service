using AWM.Service.Application.Features.Workflow.Supervisors.Commands.ApproveSupervisors;
using AWM.Service.Application.Features.Workflow.Supervisors.Commands.RemoveSupervisor;
using AWM.Service.Application.Features.Workflow.Supervisors.Commands.UpdateSupervisorWorkload;
using AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetApprovedSupervisors;
using AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetDepartmentTeachers;
using AWM.Service.WebAPI.Contracts.Supervisors;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing department supervisors and their workloads.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/departments/{departmentId}/supervisors")]
[ApiController]
public sealed class SupervisorsController : BaseController
{
    private readonly ISender _sender;

    public SupervisorsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all department teachers available to be appointed as supervisors.
    /// </summary>
    /// <param name="departmentId">The ID of the department.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of available teachers.</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableTeachers(int departmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDepartmentTeachersQuery(departmentId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TeacherResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Gets currently approved supervisors for a department/semester/speciality.
    /// </summary>
    /// <param name="departmentId">The ID of the department.</param>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="specialityId">Optional ID of the speciality.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of approved supervisors with their workloads.</returns>
    [HttpGet("approved")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApprovedSupervisors(
        int departmentId, 
        [FromQuery] int semesterId, 
        [FromQuery] int? specialityId, 
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApprovedSupervisorsQuery(departmentId, semesterId, specialityId), cancellationToken);
        
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TeacherResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Approves or updates the list of supervisors for a department/semester/speciality.
    /// </summary>
    /// <param name="departmentId">The ID of the department.</param>
    /// <param name="request">The approval request containing supervisors and workloads.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveSupervisors(
        int departmentId, 
        [FromBody] ApproveSupervisorsRequest request, 
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<ApproveSupervisorsCommand>() with { DepartmentId = departmentId };

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
    /// <param name="departmentId">The ID of the department.</param>
    /// <param name="userId">The ID of the user (teacher).</param>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="specialityId">Optional ID of the speciality.</param>
    /// <param name="request">The update request containing the new workload limit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPut("{userId}/workload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkload(
        int departmentId,
        int userId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        [FromBody] UpdateSupervisorWorkloadRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSupervisorWorkloadCommand(
            departmentId,
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
    /// <param name="departmentId">The ID of the department.</param>
    /// <param name="userId">The ID of the user (teacher).</param>
    /// <param name="semesterId">The ID of the semester.</param>
    /// <param name="specialityId">Optional ID of the speciality.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSupervisor(
        int departmentId,
        int userId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveSupervisorCommand(
            departmentId,
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
