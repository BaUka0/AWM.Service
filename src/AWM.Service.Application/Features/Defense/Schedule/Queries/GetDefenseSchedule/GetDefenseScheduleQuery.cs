namespace AWM.Service.Application.Features.Defense.Schedule.Queries.GetDefenseSchedule;

using AWM.Service.Application.Features.Defense.Schedule.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve the defense schedule slots for a GAK commission.
/// </summary>
public sealed record GetDefenseScheduleQuery : IRequest<Result<IReadOnlyList<ScheduleDto>>>
{
    /// <summary>
    /// Commission ID to get the schedule for.
    /// </summary>
    public int CommissionId { get; init; }
}
