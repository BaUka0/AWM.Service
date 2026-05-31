using AWM.Service.Application.Features.Defense.EvaluationCriteria.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Queries.GetCriteria;

public sealed record GetCriteriaQuery(
    int WorkTypeId,
    int? OrgUnitId = null,
    int? SpecialityId = null,
    int? DefenseStageType = null) : IRequest<Result<IReadOnlyList<EvaluationCriteriaDto>>>;
