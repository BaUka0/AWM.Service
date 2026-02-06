namespace AWM.Service.WebAPI.Common.Contracts.Requests.Departments;

/// <summary>
/// Request model for creating a department.
/// </summary>
public record CreateDepartmentRequest
{
    /// <summary>
    /// Institute ID to which the department belongs.
    /// </summary>
    public int InstituteId { get; init; }

    /// <summary>
    /// Name of the department.
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Optional code/abbreviation for the department.
    /// </summary>
    public string? Code { get; init; }
}
