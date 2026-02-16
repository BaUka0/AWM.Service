namespace AWM.Service.WebAPI.Common.Contracts.Responses.Org;

/// <summary>
/// Response contract for institute data.
/// </summary>
public sealed record InstituteResponse
{
    /// <summary>
    /// Institute ID.
    /// </summary>
    /// <example>1</example>
    public int Id { get; init; }

    /// <summary>
    /// University ID to which the institute belongs.
    /// </summary>
    /// <example>1</example>
    public int UniversityId { get; init; }

    /// <summary>
    /// Institute name.
    /// </summary>
    /// <example>Faculty of Computer Science</example>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Date and time when the institute was created.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who created the institute.
    /// </summary>
    /// <example>1</example>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date and time when the institute was last modified (nullable).
    /// </summary>
    /// <example>2024-02-01T14:20:00Z</example>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID who last modified the institute (nullable).
    /// </summary>
    /// <example>2</example>
    public int? LastModifiedBy { get; init; }

    /// <summary>
    /// List of departments belonging to this institute (optional, included based on query parameter).
    /// </summary>
    public IReadOnlyCollection<DepartmentResponse>? Departments { get; init; }
}