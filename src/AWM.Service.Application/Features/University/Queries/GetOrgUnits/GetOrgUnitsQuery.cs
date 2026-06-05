namespace AWM.Service.Application.Features.University.Queries.GetOrgUnits;

using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public record GetOrgUnitsQuery(int TypeId) : IRequest<Result<IReadOnlyList<OrgUnitDto>>>;
