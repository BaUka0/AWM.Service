using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.DeleteCriteria;

public sealed record DeleteCriteriaCommand(int Id) : IRequest<Result>;
