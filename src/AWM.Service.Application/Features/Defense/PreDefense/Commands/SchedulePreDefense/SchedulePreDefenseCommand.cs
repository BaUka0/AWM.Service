namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.SchedulePreDefense;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a secretary to schedule a student work for a pre-defense slot.
/// Creates a new Schedule and a linked PreDefenseAttempt.
/// </summary>
public sealed record SchedulePreDefenseCommand : IRequest<Result<long>>
{
    /// <summary>
    /// Commission ID (must be of type PreDefense).
    /// </summary>
    public int CommissionId { get; init; }

    /// <summary>
    /// StudentWork ID to schedule.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// Date and time of the pre-defense.
    /// </summary>
    public DateTime DefenseDate { get; init; }

    /// <summary>
    /// Physical or virtual location. Optional.
    /// </summary>
    public string? Location { get; init; }
}
