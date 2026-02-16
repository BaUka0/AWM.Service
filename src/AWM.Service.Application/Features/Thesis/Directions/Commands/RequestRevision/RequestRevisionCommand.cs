namespace AWM.Service.Application.Features.Thesis.Directions.Commands.RequestRevision;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to request revision of a direction (department action).
/// </summary>
public sealed record RequestRevisionCommand : IRequest<Result>
{
    /// <summary>
    /// Direction ID to request revision for.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// User ID who requests revision (department head or authorized person).
    /// </summary>
    public int RequestedBy { get; init; }

    /// <summary>
    /// Revision request comment (required - supervisor needs to know what to fix).
    /// </summary>
    public string Comment { get; init; } = string.Empty;
}