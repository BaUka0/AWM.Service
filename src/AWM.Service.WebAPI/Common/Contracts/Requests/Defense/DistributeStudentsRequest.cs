namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for distributing students to pre-defense commissions.
/// </summary>
public sealed record DistributeStudentsRequest
{
    /// <summary>
    /// Department ID.
    /// </summary>
    /// <example>1</example>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    /// <example>2</example>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Pre-defense round number (1, 2, or 3).
    /// </summary>
    /// <example>1</example>
    public int PreDefenseNumber { get; init; } = 1;
}
