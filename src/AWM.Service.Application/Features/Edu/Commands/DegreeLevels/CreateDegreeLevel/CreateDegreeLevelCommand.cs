namespace AWM.Service.Application.Features.Edu.Commands.DegreeLevels.CreateDegreeLevel;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new degree level.
/// </summary>
public sealed record CreateDegreeLevelCommand : IRequest<Result<int>>
{
    /// <summary>
    /// Degree level name (e.g. "Bachelor", "Master", "PhD").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Duration in years (e.g. 4 for Bachelor, 2 for Master, 3 for PhD).
    /// </summary>
    public int DurationYears { get; init; }

    /// <summary>
    /// User ID who creates this degree level.
    /// </summary>
    public int CreatedBy { get; init; }
}