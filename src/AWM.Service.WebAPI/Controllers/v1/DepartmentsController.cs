using AWM.Service.Application.Features.Org.Commands.Departments.CreateDepartment;
using AWM.Service.Application.Features.Org.Commands.Departments.UpdateDepartment;
using AWM.Service.Application.Features.Org.Queries.Departments.GetDepartmentsByInstitute;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Departments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing Departments.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class DepartmentsController : BaseController
{
    private readonly ISender _sender;

    public DepartmentsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get all departments for a specific institute.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <returns>List of departments</returns>
    [HttpGet]
    [Route("~/api/v{version:apiVersion}/institutes/{instituteId}/departments")]
    [RequireInstitutePermission(Permission.Institute_Manage)]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Org.DTOs.DepartmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByInstituteId(int instituteId)
    {
        var query = new GetDepartmentsByInstituteQuery
        {
            InstituteId = instituteId
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new department.
    /// </summary>
    /// <param name="request">Create department request</param>
    /// <returns>Created department ID</returns>
    [HttpPost]
    [RequireInstitutePermission(Permission.Department_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var command = new CreateDepartmentCommand
        {
            InstituteId = request.InstituteId,
            Name = request.Name,
            Code = request.Code
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(
            nameof(GetByInstituteId),
            new { instituteId = request.InstituteId, version = "1.0" },
            result.Value);
    }

    /// <summary>
    /// Update an existing department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="request">Update department request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{departmentId}")]
    [RequireDepartmentPermission(Permission.Department_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int departmentId, [FromBody] UpdateDepartmentRequest request)
    {
        var command = new UpdateDepartmentCommand
        {
            DepartmentId = departmentId,
            Name = request.Name,
            Code = request.Code
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete a department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{departmentId}")]
    [RequireDepartmentPermission(Permission.Department_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Delete(int departmentId)
    {
        // TODO: Implement DeleteDepartmentCommand when available
        return StatusCode(StatusCodes.Status501NotImplemented, new { Message = "Delete department not yet implemented" });
    }
}

