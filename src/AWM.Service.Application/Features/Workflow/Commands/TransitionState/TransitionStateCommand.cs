namespace AWM.Service.Application.Features.Workflow.Commands.TransitionState;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record TransitionStateCommand : IRequest<Result>
{
    /// <summary>
    /// The type of entity (e.g., "Direction", "StudentWork").
    /// </summary>
    public string EntityType { get; init; } = null!;

    /// <summary>
    /// The ID of the entity to transition.
    /// </summary>
    public long EntityId { get; init; }

    /// <summary>
    /// The target state ID to transition to.
    /// </summary>
    public int TargetStateId { get; init; }

    /// <summary>
    /// Optional comment for the transition.
    /// </summary>
    public string? Comment { get; init; }
}
