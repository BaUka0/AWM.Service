namespace AWM.Service.Application.Features.Defense.Schedule.Commands.CreateDefenseSchedule;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new defense schedule slot for a GAK commission.
/// </summary>
public sealed record CreateDefenseScheduleCommand : IRequest<Result<long>>
{
    /// <summary>
    /// Commission ID (must be of type GAK).
    /// </summary>
    public int CommissionId { get; init; }

    /// <summary>
    /// Date and time of the defense.
    /// </summary>
    public DateTime DefenseDate { get; init; }

    /// <summary>
    /// Physical or virtual location. Optional.
    /// </summary>
    public string? Location { get; init; }
}
