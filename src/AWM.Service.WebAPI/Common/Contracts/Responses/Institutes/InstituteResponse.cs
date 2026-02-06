namespace AWM.Service.WebAPI.Common.Contracts.Responses.Institutes;

/// <summary>
/// Response model for institute data.
/// </summary>
public record InstituteResponse
{
    /// <summary>
    /// Institute ID.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Institute name.
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// University ID to which the institute belongs.
    /// </summary>
    public int UniversityId { get; init; }

    /// <summary>
    /// List of departments (if included).
    /// </summary>
    public IReadOnlyList<DepartmentSummaryResponse>? Departments { get; init; }
}

/// <summary>
/// Summary response for department (used in nested responses).
/// </summary>
public record DepartmentSummaryResponse
{
    /// <summary>
    /// Department ID.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Department name.
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Department code.
    /// </summary>
    public string? Code { get; init; }
}
