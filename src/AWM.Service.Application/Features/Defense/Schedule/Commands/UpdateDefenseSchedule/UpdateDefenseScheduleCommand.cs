namespace AWM.Service.Application.Features.Defense.Schedule.Commands.UpdateDefenseSchedule;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update (reschedule) an existing defense schedule slot.
/// </summary>
public sealed record UpdateDefenseScheduleCommand : IRequest<Result>
{
    /// <summary>
    /// Schedule ID to update.
    /// </summary>
    public long ScheduleId { get; init; }

    /// <summary>
    /// New defense date. Optional — if null, date is not changed.
    /// </summary>
    public DateTime? DefenseDate { get; init; }

    /// <summary>
    /// New location. Optional — if null, location is not changed.
    /// </summary>
    public string? Location { get; init; }
}
