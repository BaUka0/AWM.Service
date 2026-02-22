namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for creating a student work.
/// </summary>
public sealed record CreateStudentWorkRequest
{
    /// <summary>
    /// ID of the topic the work is based on.
    /// </summary>
    /// <example>1</example>
    public long TopicId { get; init; }

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
}
