namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Org.Departments.Queries.GetDepartmentsByInstitute;
using AWM.Service.WebAPI.Authorization;
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
    /// Get all departments.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of departments</returns>
    [HttpGet]
    [Route("~/api/v{version:apiVersion}/departments")]
    [RequireAccess("Org_Departments", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new Application.Features.Org.Departments.Queries.GetAllDepartments.GetAllDepartmentsQuery();

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<DepartmentResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get all departments for a specific institute.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of departments</returns>
    [HttpGet]
    [Route("~/api/v{version:apiVersion}/institutes/{instituteId}/departments")]
    [RequireAccess("Org_Departments", "Read")]
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
}
