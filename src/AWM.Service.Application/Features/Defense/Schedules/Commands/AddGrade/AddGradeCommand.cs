using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.AddGrade;

public sealed record AddGradeCommand(
    long ScheduleId,
    int CriteriaId,
    int Score,
    string? Comment = null) : IRequest<Result<long>>;
