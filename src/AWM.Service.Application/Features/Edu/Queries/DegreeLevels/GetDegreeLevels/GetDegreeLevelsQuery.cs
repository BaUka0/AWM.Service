namespace AWM.Service.Application.Features.Edu.Queries.DegreeLevels.GetDegreeLevels;

using AWM.Service.Application.Features.Edu.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all degree levels.
/// </summary>
public sealed record GetDegreeLevelsQuery : IRequest<Result<IReadOnlyList<DegreeLevelDto>>>
{
    /// <summary>
    /// Filter by name (optional, partial match).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Filter by minimum duration in years (optional).
    /// </summary>
    public int? MinDurationYears { get; init; }

    /// <summary>
    /// Filter by maximum duration in years (optional).
    /// </summary>
    public int? MaxDurationYears { get; init; }
}