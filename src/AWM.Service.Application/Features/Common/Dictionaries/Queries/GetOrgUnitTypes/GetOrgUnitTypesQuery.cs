namespace AWM.Service.Application.Features.Common.Dictionaries.Queries.GetOrgUnitTypes;

using AWM.Service.Domain.University;
using MediatR;

/// <summary>
/// Query to get all org unit types (reference dictionary).
/// </summary>
public sealed record GetOrgUnitTypesQuery : IRequest<IReadOnlyList<OrgUnitType>>;
