namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Employees.Queries.GetEmployeesByDepartment; 
using AWM.Service.Application.Features.Edu.Employees.Queries.GetSupervisors;       
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Controller for managing Employees.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
[ApiController]
[Produces("application/json")]
public sealed class EmployeesController : BaseController
{
    private readonly ISender _sender;

    public EmployeesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get employees by department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of employees in the department</returns>
    [HttpGet]
    [RequireAccess("Staff", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByDepartment([FromQuery] int departmentId, CancellationToken cancellationToken = default)
    {
        var query = new GetEmployeesByDepartmentQuery
        {
            DepartmentId = departmentId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<EmployeeResponse>>();     

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
    [ProducesResponseType(typeof(IReadOnlyList<EmployeeResponse>), StatusCodes.Status200OK)]
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

        var response = result.Value.Adapt<IReadOnlyList<EmployeeResponse>>();     

        return Ok(response);
    }
}
