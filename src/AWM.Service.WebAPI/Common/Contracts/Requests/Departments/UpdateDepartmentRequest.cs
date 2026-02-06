namespace AWM.Service.WebAPI.Common.Contracts.Requests.Departments;

/// <summary>
/// Request model for updating a department.
/// </summary>
public record UpdateDepartmentRequest
{
    /// <summary>
    /// New name for the department.
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Optional code/abbreviation for the department.
    /// </summary>
    public string? Code { get; init; }
}
