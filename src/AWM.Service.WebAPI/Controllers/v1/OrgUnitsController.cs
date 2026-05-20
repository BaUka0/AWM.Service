namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Org.OrgUnits.Queries.GetAllOrgUnits;
using AWM.Service.Application.Features.Org.OrgUnits.Queries.GetOrgUnitById;
using AWM.Service.Application.Features.Org.OrgUnits.Queries.GetOrgUnitChildren;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses.Org;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing organizational units (institutes, departments, etc.).
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
    /// Get all organizational units with optional type filter.
    /// </summary>
    /// <param name="typeId">Optional OrgUnitType ID filter (from /dictionaries/org-unit-types).</param>
    [HttpGet]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<OrgUnitResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? typeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllOrgUnitsQuery { TypeId = typeId };
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<IReadOnlyList<OrgUnitResponse>>());
    }

    /// <summary>
    /// Get a specific organizational unit by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(OrgUnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetOrgUnitByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<OrgUnitResponse>());
    }

    /// <summary>
    /// Get children of an organizational unit (e.g., departments of an institute).
    /// </summary>
    [HttpGet("{id:int}/children")]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<OrgUnitResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildren(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetOrgUnitChildrenQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailed) return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<IReadOnlyList<OrgUnitResponse>>());
    }
}
