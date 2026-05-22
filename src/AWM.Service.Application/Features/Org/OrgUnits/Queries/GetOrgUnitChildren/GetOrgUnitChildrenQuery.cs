namespace AWM.Service.Application.Features.Org.OrgUnits.Queries.GetOrgUnitChildren;

using AWM.Service.Application.Features.Org.OrgUnits.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get children of an organizational unit.
/// </summary>
public sealed record GetOrgUnitChildrenQuery(int ParentId) : IRequest<Result<IReadOnlyList<OrgUnitDto>>>;
