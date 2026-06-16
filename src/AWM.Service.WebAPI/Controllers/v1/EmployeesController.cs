using AWM.Service.Application.Features.Workflow.Employees.Commands.ApproveEmployees;
using AWM.Service.Application.Features.Workflow.Employees.Commands.ConfirmEmployees;
using AWM.Service.Application.Features.Workflow.Employees.Commands.UnlockEmployees;
using AWM.Service.Application.Features.Workflow.Employees.Commands.RemoveEmployee;
using AWM.Service.Application.Features.Workflow.Employees.Commands.UpdateEmployeeWorkload;
using AWM.Service.Application.Features.Workflow.Employees.Queries.GetApprovedEmployees;
using AWM.Service.Application.Features.Workflow.Employees.Queries.GetOrgUnitEmployees;
using AWM.Service.Application.Features.Workflow.Employees.Queries.GetEmployeesStatus;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests;
using AWM.Service.WebAPI.Common.Contracts.Responses;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/org-units/{orgUnitId}/employees")]
[ApiController]
[Authorize]
public sealed class EmployeesController : BaseController
{
    private readonly ISender _sender;

    public EmployeesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("available")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableEmployees(int orgUnitId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrgUnitEmployeesQuery(orgUnitId), cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TeacherResponse>>();
        return Ok(response);
    }

    [HttpGet("approved")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApprovedEmployees(
        int orgUnitId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApprovedEmployeesQuery(orgUnitId, semesterId, specialityId), cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<TeacherResponse>>();
        return Ok(response);
    }

    [HttpPost("approve")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveEmployees(
        int orgUnitId,
        [FromBody] ApproveEmployeesRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<ApproveEmployeesCommand>() with { OrgUnitId = orgUnitId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    [HttpPut("{userId}/workload")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkload(
        int orgUnitId,
        int userId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        [FromBody] UpdateEmployeeWorkloadRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEmployeeWorkloadCommand(
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

    [HttpDelete("{userId}")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveEmployee(
        int orgUnitId,
        int userId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveEmployeeCommand(
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

    [HttpGet("status")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(typeof(EmployeesStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeesStatus(
        int orgUnitId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmployeesStatusQuery(orgUnitId, semesterId, specialityId), cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<EmployeesStatusResponse>();
        return Ok(response);
    }

    [HttpPost("confirm")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmployees(
        int orgUnitId,
        [FromBody] ConfirmEmployeesRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmEmployeesCommand(orgUnitId, request.SemesterId, request.SpecialityId);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    [HttpPost("unlock")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnlockEmployees(
        int orgUnitId,
        [FromBody] ConfirmEmployeesRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UnlockEmployeesCommand(orgUnitId, request.SemesterId, request.SpecialityId);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }
}
