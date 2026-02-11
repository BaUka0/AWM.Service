namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for creating a research direction.
/// </summary>
public sealed record CreateDirectionRequest
{
    /// <summary>
    /// Department ID.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Supervisor ID (научный руководитель).
    /// </summary>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Work type ID (Bachelor thesis, Master thesis, etc.).
    /// </summary>
    public int WorkTypeId { get; init; }

    /// <summary>
    /// Title in Russian (required).
    /// </summary>
    public string TitleRu { get; init; } = string.Empty;

    /// <summary>
    /// Title in Kazakh (optional).
    /// </summary>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Title in English (optional).
    /// </summary>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Direction description (optional).
    /// </summary>
    public string? Description { get; init; }
}