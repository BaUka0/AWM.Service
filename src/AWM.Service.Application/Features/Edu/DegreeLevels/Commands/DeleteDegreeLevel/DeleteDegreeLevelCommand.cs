namespace AWM.Service.Application.Features.Edu.DegreeLevels.Commands.DeleteDegreeLevel;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to soft delete a degree level.
/// </summary>
public sealed record DeleteDegreeLevelCommand : IRequest<Result>
{
    public int Id { get; init; }
}
