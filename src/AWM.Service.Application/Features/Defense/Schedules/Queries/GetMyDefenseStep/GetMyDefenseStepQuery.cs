using AWM.Service.Application.Features.Defense.Schedules.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GetMyDefenseStep;

public sealed record GetMyDefenseStepQuery() : IRequest<Result<DefenseStepDto>>;
