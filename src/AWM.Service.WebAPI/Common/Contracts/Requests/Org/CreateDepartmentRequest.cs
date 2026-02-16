namespace AWM.Service.WebAPI.Common.Contracts.Requests.Org;

/// <summary>
/// Request contract for creating a new department.
/// </summary>
public sealed record CreateDepartmentRequest
{
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
}