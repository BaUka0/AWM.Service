namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

/// <summary>
/// Request contract for creating an academic program.
/// </summary>
public sealed record CreateAcademicProgramRequest
{
    /// <summary>
    /// Department ID (foreign key).
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Degree level ID (Bachelor, Master, PhD).
    /// </summary>
    public int DegreeLevelId { get; init; }

    /// <summary>
    /// Program code (optional, e.g. "CS-B-2024").
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Program name (required, e.g. "Computer Science").
    /// </summary>
    public string Name { get; init; } = string.Empty;
}