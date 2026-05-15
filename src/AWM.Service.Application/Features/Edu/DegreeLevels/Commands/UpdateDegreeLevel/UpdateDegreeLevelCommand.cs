namespace AWM.Service.Application.Features.Edu.DegreeLevels.Commands.UpdateDegreeLevel;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update an existing degree level.
/// </summary>
public sealed record UpdateDegreeLevelCommand : IRequest<Result>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DurationYears { get; init; }
}
