namespace AWM.Service.Application.Features.Org.OrgUnits.Queries.GetOrgUnitById;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get a single organizational unit by ID.
/// </summary>
public sealed record GetOrgUnitByIdQuery(int Id) : IRequest<Result<OrgUnitDto>>;
