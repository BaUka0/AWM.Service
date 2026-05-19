namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Staff.Queries.GetStaffByDepartment;
using AWM.Service.Application.Features.Edu.Staff.Queries.GetSupervisors;
using AWM.Service.WebAPI.Authorization;
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
    [RequireAccess("Staff", "Read")]
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
    /// Get supervisors for a department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of approved supervisors in the department</returns>
    [HttpGet("supervisors")]
    [RequireAccess("Staff", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<StaffResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSupervisors([FromQuery] int departmentId, CancellationToken cancellationToken = default)
    {
        var query = new GetSupervisorsQuery
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
}
