namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Staff.Commands.ApproveSupervisors;
using AWM.Service.Application.Features.Edu.Staff.Commands.CreateStaff;
using AWM.Service.Application.Features.Edu.Staff.Commands.UpdateStaff;
using AWM.Service.Application.Features.Edu.Staff.Queries.GetStaffByDepartment;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Staff members.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class StaffController : BaseController
{
    private readonly ISender _sender;

    public StaffController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get staff members by department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of staff in the department</returns>
    [HttpGet]
    [RequireDepartmentPermission(Permission.Staff_View)]
    [ProducesResponseType(typeof(IReadOnlyList<StaffResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByDepartment([FromQuery] int departmentId, CancellationToken cancellationToken = default)
    {
        var query = new GetStaffByDepartmentQuery
        {
            DepartmentId = departmentId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<StaffResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Create a new staff member.
    /// </summary>
    /// <param name="request">Create staff request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created staff ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Staff_Create)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateStaffCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetByDepartment), new { departmentId = request.DepartmentId, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing staff member.
    /// </summary>
    /// <param name="staffId">Staff ID</param>
    /// <param name="request">Update staff request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPut("{staffId}")]
    [RequireDepartmentPermission(Permission.Staff_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int staffId, [FromBody] UpdateStaffRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UpdateStaffCommand>() with { StaffId = staffId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
    /// <summary>
    /// Approves selected staff members as supervisors in a department.
    /// </summary>
    /// <param name="request">Request containing department ID and list of staff IDs to approve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("approve-supervisors")]
    [RequireDepartmentPermission(Permission.Staff_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveSupervisors([FromBody] ApproveSupervisorsRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<ApproveSupervisorsCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
