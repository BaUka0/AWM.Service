namespace AWM.Service.Application.Features.Edu.SpecialityLevels.Queries.GetSpecialityLevels;

using AWM.Service.Application.Features.Edu.SpecialityLevels.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all degree levels.
/// </summary>
public sealed record GetSpecialityLevelsQuery : IRequest<Result<IReadOnlyList<SpecialityLevelDto>>>
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
