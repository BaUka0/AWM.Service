namespace AWM.Service.WebAPI.Common.Contracts.Responses.Org;

/// <summary>
/// Response contract for department data.
/// </summary>
public sealed record DepartmentResponse
{
    /// <summary>
    /// Department ID.
    /// </summary>
    /// <example>1</example>
    public int Id { get; init; }

    /// <summary>
    /// Institute ID to which the department belongs.
    /// </summary>
    /// <example>1</example>
    public int InstituteId { get; init; }

    /// <summary>
    /// Department name.
    /// </summary>
    /// <example>Department of Software Engineering</example>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Department code (optional).
    /// </summary>
    /// <example>SE</example>
    public string? Code { get; init; }

    /// <summary>
    /// Date and time when the department was created.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who created the department.
    /// </summary>
    /// <example>1</example>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date and time when the department was last modified (nullable).
    /// </summary>
    /// <example>2024-02-01T14:20:00Z</example>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID who last modified the department (nullable).
    /// </summary>
    /// <example>2</example>
    public int? LastModifiedBy { get; init; }
}