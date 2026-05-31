using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.CreateCriteria;

public sealed record CreateCriteriaCommand(
    int WorkTypeId,
    string CriteriaName,
    int MaxScore,
    decimal Weight = 1.0m,
    int? OrgUnitId = null,
    int? SpecialityId = null,
    int? DefenseStageType = null,
    int SortOrder = 0) : IRequest<Result<int>>;
