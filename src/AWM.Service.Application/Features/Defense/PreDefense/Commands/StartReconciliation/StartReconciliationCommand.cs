namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.StartReconciliation;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record StartReconciliationCommand : IRequest<Result>
{
    public long ScheduleId { get; init; }
}
