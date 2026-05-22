namespace AWM.Service.Application.Features.Org.OrgUnits.Queries.GetAllOrgUnits;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all organizational units with optional type filter.
/// </summary>
public sealed record GetAllOrgUnitsQuery : IRequest<Result<IReadOnlyList<OrgUnitDto>>>
{
    /// <summary>
    /// Filter by OrgUnitType ID (optional).
    /// </summary>
    public int? TypeId { get; init; }
}
