using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.GenerateSchedule;

public sealed record GenerateScheduleCommand(
    int CommissionId,
    DateTime StartDate,
    string? Location,
    int SlotDurationMinutes,
    IReadOnlyList<long> WorkIds) : IRequest<Result>;
