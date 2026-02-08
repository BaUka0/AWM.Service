using AWM.Service.Application.Features.Edu.Commands.Staff.CreateStaff;
using AWM.Service.Application.Features.Edu.Commands.Staff.UpdateStaff;
using AWM.Service.Application.Features.Edu.Queries.Staff.GetStaffByDepartment;
using AWM.Service.Application.Features.Edu.Queries.Staff.GetSupervisors;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing Staff profiles.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class StaffController : BaseController
{
    private readonly ISender _sender;

    public StaffController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get staff by department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <returns>List of staff</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Edu.DTOs.StaffDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] int departmentId)
    {
        var query = new GetStaffByDepartmentQuery { DepartmentId = departmentId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get supervisors (staff with capacity) for a department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <returns>List of supervisors</returns>
    [HttpGet("supervisors")]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Edu.DTOs.StaffDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSupervisors([FromQuery] int departmentId)
    {
        var query = new GetSupervisorsQuery { DepartmentId = departmentId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new staff profile.
    /// </summary>
    /// <param name="request">Create staff request</param>
    /// <returns>Created staff ID</returns>
    [HttpPost]
    [RequirePermission(Permission.Users_Create)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)
    {
        var command = new CreateStaffCommand
        {
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            Position = request.Position,
            AcademicDegree = request.AcademicDegree,
            MaxStudentsLoad = request.MaxStudentsLoad
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetAll), new { departmentId = request.DepartmentId, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing staff profile.
    /// </summary>
    /// <param name="staffId">Staff ID</param>
    /// <param name="request">Update staff request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{staffId}")]
    [RequirePermission(Permission.Users_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int staffId, [FromBody] UpdateStaffRequest request)
    {
        var command = new UpdateStaffCommand
        {
            StaffId = staffId,
            Position = request.Position,
            AcademicDegree = request.AcademicDegree,
            MaxStudentsLoad = request.MaxStudentsLoad,
            DepartmentId = request.DepartmentId
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
