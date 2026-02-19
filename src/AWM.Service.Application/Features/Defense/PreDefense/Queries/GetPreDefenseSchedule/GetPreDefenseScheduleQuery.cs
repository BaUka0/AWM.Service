namespace AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseSchedule;

using AWM.Service.Application.Features.Defense.PreDefense.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve the pre-defense schedule slots for a commission.
/// </summary>
public sealed record GetPreDefenseScheduleQuery : IRequest<Result<IReadOnlyList<PreDefenseScheduleDto>>>
{
    /// <summary>
    /// Commission ID to get the schedule for.
    /// </summary>
    public int CommissionId { get; init; }
}
