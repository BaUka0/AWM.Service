namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Org.Institutes.Queries.GetAllInstitutes;
using AWM.Service.Application.Features.Org.Institutes.Queries.GetInstituteById;
using AWM.Service.Application.Features.Org.Departments.Queries.GetDepartmentsByInstitute;
using AWM.Service.Application.Features.Org.Departments.Queries.GetAllDepartments;
using AWM.Service.WebAPI.Common.Contracts.Responses.Org;
using AWM.Service.WebAPI.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Controller for managing organizational units (Institutes and Departments).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/org-units")]
[ApiController]
[Produces("application/json")]
public sealed class OrgUnitsController : BaseController
{
    private readonly ISender _sender;

    public OrgUnitsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all institutes.
    /// </summary>
    [HttpGet("institutes")]
    [HttpGet("~/api/v{version:apiVersion}/institutes")]
    [RequireAccess("Org_Institutes", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<OrgUnitResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstitutes([FromQuery] bool includeDepartments = false, CancellationToken cancellationToken = default)
    {
        var query = new GetAllInstitutesQuery { IncludeDepartments = includeDepartments };
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Select(i => new OrgUnitResponse
        {
            Id = i.Id,
            ParentId = null,
            Name = i.Name,
            TypeId = 2, // Institute
            Children = i.Departments?.Select(d => new OrgUnitResponse
            {
                Id = d.Id,
                ParentId = d.InstituteId,
                Name = d.Name,
                Code = d.Code,
                TypeId = 1 // Department
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get a specific institute by ID.
    /// </summary>
    [HttpGet("institutes/{instituteId}")]
    [HttpGet("~/api/v{version:apiVersion}/institutes/{instituteId}")]
    [RequireAccess("Org_Institutes", "Read")]
    [ProducesResponseType(typeof(OrgUnitResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstituteById(int instituteId, [FromQuery] bool includeDepartments = false, CancellationToken cancellationToken = default)
    {
        var query = new GetInstituteByIdQuery { InstituteId = instituteId, IncludeDepartments = includeDepartments };
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        var i = result.Value;
        var response = new OrgUnitResponse
        {
            Id = i.Id,
            ParentId = null,
            Name = i.Name,
            TypeId = 2, // Institute
            Children = i.Departments?.Select(d => new OrgUnitResponse
            {
                Id = d.Id,
                ParentId = d.InstituteId,
                Name = d.Name,
                Code = d.Code,
                TypeId = 1 // Department
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Get all departments.
    /// </summary>
    [HttpGet("departments")]
    [HttpGet("~/api/v{version:apiVersion}/departments")]
    [RequireAccess("Org_Departments", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<OrgUnitResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken = default)
    {
        var query = new GetAllDepartmentsQuery();
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Select(d => new OrgUnitResponse
        {
            Id = d.Id,
            ParentId = d.InstituteId,
            Name = d.Name,
            Code = d.Code,
            TypeId = 1 // Department
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get all departments belonging to a specific institute.
    /// </summary>
    [HttpGet("institutes/{instituteId}/departments")]
    [HttpGet("~/api/v{version:apiVersion}/institutes/{instituteId}/departments")]
    [RequireAccess("Org_Departments", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<OrgUnitResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartmentsByInstitute(int instituteId, CancellationToken cancellationToken = default)
    {
        var query = new GetDepartmentsByInstituteQuery { InstituteId = instituteId };
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Select(d => new OrgUnitResponse
        {
            Id = d.Id,
            ParentId = d.InstituteId,
            Name = d.Name,
            Code = d.Code,
            TypeId = 1 // Department
        }).ToList();

        return Ok(response);
    }
}
