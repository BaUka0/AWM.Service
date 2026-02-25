namespace AWM.Service.Application.Features.Defense.Schedule.Commands.AssignWorkToSlot;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to assign a student work to an existing defense schedule slot.
/// </summary>
public sealed record AssignWorkToSlotCommand : IRequest<Result>
{
    /// <summary>
    /// Schedule (slot) ID to assign work to.
    /// </summary>
    public long ScheduleId { get; init; }

    /// <summary>
    /// StudentWork ID to assign.
    /// </summary>
    public long WorkId { get; init; }
}
