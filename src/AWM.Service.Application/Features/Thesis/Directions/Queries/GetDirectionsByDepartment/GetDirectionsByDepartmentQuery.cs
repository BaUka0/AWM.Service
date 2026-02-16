using AWM.Service.Application.Features.Thesis.Directions.DTOs;

namespace AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionsByDepartment;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all directions for a department in a specific academic year.
/// </summary>
public sealed record GetDirectionsByDepartmentQuery : IRequest<Result<IReadOnlyList<DirectionDto>>>
{
    /// <summary>
    /// Department ID.
    /// </summary>
    public int DepartmentId { get; init; }

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
    /// Filter by supervisor ID (optional).
    /// </summary>
    public int? SupervisorId { get; init; }

    /// <summary>
    /// Include soft-deleted directions (default: false).
    /// </summary>
    public bool IncludeDeleted { get; init; } = false;
}