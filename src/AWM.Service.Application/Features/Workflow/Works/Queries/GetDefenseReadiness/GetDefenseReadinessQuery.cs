using AWM.Service.Application.Features.Workflow.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetDefenseReadiness;

public sealed record GetDefenseReadinessQuery(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null) : IRequest<Result<IReadOnlyList<DefenseReadinessDto>>>;
