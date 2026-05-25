using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.UpdateCriteria;

public sealed record UpdateCriteriaCommand(
    int Id,
    string CriteriaName,
    int MaxScore,
    decimal Weight) : IRequest<Result>;
