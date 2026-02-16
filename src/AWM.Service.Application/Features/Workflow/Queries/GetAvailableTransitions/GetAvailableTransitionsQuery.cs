namespace AWM.Service.Application.Features.Workflow.Queries.GetAvailableTransitions;

using AWM.Service.Application.Features.Workflow.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetAvailableTransitionsQuery : IRequest<Result<IReadOnlyList<TransitionDto>>>
{
    /// <summary>
    /// The type of entity (e.g., "Direction", "StudentWork").
    /// </summary>
    public string EntityType { get; init; } = null!;

    /// <summary>
    /// The ID of the entity.
    /// </summary>
    public long EntityId { get; init; }

    /// <summary>
    /// Optional: filter transitions by role ID.
    /// </summary>
    public int? RoleId { get; init; }
}
