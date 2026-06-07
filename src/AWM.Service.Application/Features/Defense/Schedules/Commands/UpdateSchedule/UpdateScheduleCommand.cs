using KDS.Primitives.FluentResult;
using MediatR;
using System;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.UpdateSchedule;

public sealed record UpdateScheduleCommand(
    long Id,
    int? CommissionId,
    DateTime? DefenseDate,
    string? Location) : IRequest<Result>;
