using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.DeleteSchedule;

/// <summary>
/// CQRS Command to delete (unschedule) a defense schedule slot.
/// </summary>
public sealed record DeleteScheduleCommand(long Id) : IRequest<Result>;
