using AWM.Service.Application.Features.Workflow.Stages.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Stages.Queries.GetOrgUnitSpecialities;

/// <summary>
/// Query to get all unique specialities associated with an OrgUnit (department).
/// </summary>
public sealed record GetOrgUnitSpecialitiesQuery(int? OrgUnitId = null) : IRequest<Result<IReadOnlyList<SpecialityDto>>>;
