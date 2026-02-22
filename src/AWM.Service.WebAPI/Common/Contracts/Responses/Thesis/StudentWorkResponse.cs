namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for student work data (list view).
/// </summary>
public sealed record StudentWorkResponse
{
    /// <summary>
    /// Work ID.
    /// </summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>
    /// Topic ID (nullable for standalone works).
    /// </summary>
    /// <example>10</example>
    public long? TopicId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    /// <example>2</example>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Department ID.
    /// </summary>
    /// <example>1</example>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Current workflow state ID.
    /// </summary>
    /// <example>1</example>
    public int CurrentStateId { get; init; }

    /// <summary>
    /// Whether the work has been defended.
    /// </summary>
    /// <example>false</example>
    public bool IsDefended { get; init; }

    /// <summary>
    /// Final grade (if defended).
    /// </summary>
    public string? FinalGrade { get; init; }

    /// <summary>
    /// Date when the work was created.
    /// </summary>
    /// <example>2024-09-01T10:00:00Z</example>
    public DateTime CreatedAt { get; init; }
}
