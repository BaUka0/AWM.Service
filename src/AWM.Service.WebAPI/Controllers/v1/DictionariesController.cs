namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Admin.Roles.Queries.GetAllRoles;
using AWM.Service.Application.Features.Common.Dictionaries.Queries.GetSemesterTypes;
using AWM.Service.Application.Features.Common.Dictionaries.Queries.GetOrgUnitTypes;
using AWM.Service.Application.Features.Common.Dictionaries.Queries.GetWorkflowStages;
using AWM.Service.Application.Features.Edu.AcademicPrograms.Queries.GetAcademicPrograms;
using AWM.Service.Application.Features.Edu.DegreeLevels.Queries.GetDegreeLevels;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for read-only reference dictionaries.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dictionaries")]
[ApiController]
[Produces("application/json")]
public sealed class DictionariesController : BaseController
{
    private readonly ISender _sender;

    public DictionariesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all degree levels (speciality levels).
    /// </summary>
    [HttpGet("degree-levels")]
    [RequireAccess("Specialities", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DictionaryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDegreeLevels(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetDegreeLevelsQuery(), cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Select(d => new DictionaryItemResponse { Id = d.Id, Name = d.Name }).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Get all academic programs (specialities).
    /// </summary>
    [HttpGet("academic-programs")]
    [RequireAccess("Specialities", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DictionaryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAcademicPrograms(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetAcademicProgramsQuery(), cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value
            .Select(d => new DictionaryItemResponse { Id = d.Id, Name = d.Name ?? string.Empty, Code = d.Code })
            .ToList();
        return Ok(response);
    }

    /// <summary>
    /// Get all system roles.
    /// </summary>
    [HttpGet("roles")]
    [RequireAccess("Users_Roles", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DictionaryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetAllRolesQuery(), cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value
            .Select(d => new DictionaryItemResponse { Id = d.RoleId, Name = d.DisplayName, Code = d.SystemName })
            .ToList();
        return Ok(response);
    }

    /// <summary>
    /// Get all semester types.
    /// </summary>
    [HttpGet("semester-types")]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DictionaryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSemesterTypes(CancellationToken cancellationToken = default)
    {
        var items = await _sender.Send(new GetSemesterTypesQuery(), cancellationToken);
        var response = items.Select(d => new DictionaryItemResponse { Id = d.Id, Name = d.Title }).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Get all workflow stages.
    /// </summary>
    [HttpGet("workflow-stages")]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DictionaryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkflowStages(CancellationToken cancellationToken = default)
    {
        var items = await _sender.Send(new GetWorkflowStagesQuery(), cancellationToken);
        var response = items.Select(d => new DictionaryItemResponse { Id = d.Id, Name = d.Name }).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Get all org unit types.
    /// </summary>
    [HttpGet("org-unit-types")]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<DictionaryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrgUnitTypes(CancellationToken cancellationToken = default)
    {
        var items = await _sender.Send(new GetOrgUnitTypesQuery(), cancellationToken);
        var response = items.Select(d => new DictionaryItemResponse { Id = d.Id, Name = d.Title }).ToList();
        return Ok(response);
    }
}
