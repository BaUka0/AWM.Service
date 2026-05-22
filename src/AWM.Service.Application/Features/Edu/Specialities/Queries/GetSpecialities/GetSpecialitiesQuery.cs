namespace AWM.Service.Application.Features.Edu.Specialities.Queries.GetSpecialities;

using AWM.Service.Application.Features.Edu.Specialities.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get academic programs with optional filtering.
/// </summary>
public sealed record GetSpecialitiesQuery : IRequest<Result<IReadOnlyList<SpecialityDto>>>
{
    /// <summary>
    /// Filter by department ID (optional).
    /// </summary>
    public int? DepartmentId { get; init; }

    /// <summary>
    /// Filter by degree level ID (optional).
    /// </summary>
    public int? DegreeLevelId { get; init; }

    /// <summary>
    /// Filter by program code (optional, partial match).
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Filter by program name (optional, partial match).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Include soft-deleted programs (default: false).
    /// </summary>
    public bool IncludeDeleted { get; init; } = false;
}
