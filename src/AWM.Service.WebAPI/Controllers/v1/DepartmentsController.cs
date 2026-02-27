namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Org.Departments.Commands.CreateDepartment;
using AWM.Service.Application.Features.Org.Departments.Commands.DeleteDepartment;
using AWM.Service.Application.Features.Org.Departments.Commands.UpdateDepartment;
using AWM.Service.Application.Features.Org.Departments.Queries.GetDepartmentsByInstitute;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Departments;
using AWM.Service.WebAPI.Common.Contracts.Responses.Org;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Departments.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class DepartmentsController : BaseController
{
    private readonly ISender _sender;

    public DepartmentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all departments for a specific institute.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of departments</returns>
    [HttpGet]
    [Route("~/api/v{version:apiVersion}/institutes/{instituteId}/departments")]
    [RequirePermission(Permission.Departments_View)]
    [ProducesResponseType(typeof(IReadOnlyList<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByInstituteId(int instituteId, CancellationToken cancellationToken = default)
    {
        var query = new GetDepartmentsByInstituteQuery
        {
            InstituteId = instituteId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<DepartmentResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Create a new department.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <param name="request">Create department request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created department ID</returns>
    [HttpPost]
    [Route("~/api/v{version:apiVersion}/institutes/{instituteId}/departments")]
    [RequireInstitutePermission(Permission.Department_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromRoute] int instituteId, [FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateDepartmentCommand>() with { InstituteId = instituteId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(
            nameof(GetByInstituteId),
            new { instituteId, version = "1.0" },
            result.Value);
    }

    /// <summary>
    /// Update an existing department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="request">Update department request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPut("{departmentId}")]
    [RequireDepartmentPermission(Permission.Department_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int departmentId, [FromBody] UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UpdateDepartmentCommand>() with { DepartmentId = departmentId };

        var result = await _sender.Send(command, cancellationToken);

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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{departmentId}")]
    [RequireDepartmentPermission(Permission.Department_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int departmentId, CancellationToken cancellationToken = default)
    {
        var command = new DeleteDepartmentCommand
        {
            DepartmentId = departmentId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
