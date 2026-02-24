namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for retrieving directions by department.
/// </summary>
public sealed record GetDirectionsByDepartmentRequest
{
    /// <summary>
    /// Department ID (required).
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID (required).
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Filter by work type ID (optional).
    /// </summary>
    public int? WorkTypeId { get; init; }

    /// <summary>
    /// Filter by state ID (optional).
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
