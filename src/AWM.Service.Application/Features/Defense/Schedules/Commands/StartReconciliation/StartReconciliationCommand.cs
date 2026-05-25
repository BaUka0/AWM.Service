using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.StartReconciliation;

public sealed record StartReconciliationCommand(long ScheduleId) : IRequest<Result>;
