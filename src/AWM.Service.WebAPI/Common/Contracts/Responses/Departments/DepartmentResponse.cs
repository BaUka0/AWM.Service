namespace AWM.Service.WebAPI.Common.Contracts.Responses.Departments;

/// <summary>
/// Response model for department data.
/// </summary>
public record DepartmentResponse
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
    /// Department code/abbreviation.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Institute ID to which the department belongs.
    /// </summary>
    public int InstituteId { get; init; }

    /// <summary>
    /// Institute name (for display purposes).
    /// </summary>
    public string? InstituteName { get; init; }
}
