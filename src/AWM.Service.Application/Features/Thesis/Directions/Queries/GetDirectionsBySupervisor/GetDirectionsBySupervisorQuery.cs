namespace AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionsBySupervisor;

using AWM.Service.Application.Features.Thesis.Directions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all directions created by a specific supervisor.
/// </summary>
public sealed record GetDirectionsBySupervisorQuery : IRequest<Result<IReadOnlyList<DirectionDto>>>
{
    /// <summary>
    /// Supervisor ID (научный руководитель).
    /// </summary>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Filter by work type ID (optional).
    /// </summary>
    public int? WorkTypeId { get; init; }

    /// <summary>
    /// Filter by current state ID (optional).
    /// </summary>
    public int? StateId { get; init; }

    /// <summary>
    /// Include soft-deleted directions (default: false).
    /// </summary>
    public bool IncludeDeleted { get; init; } = false;
}