namespace AWM.Service.WebAPI.Common.Contracts.Requests.Org;

/// <summary>
/// Request contract for updating an existing department.
/// </summary>
public sealed record UpdateDepartmentRequest
{
    /// <summary>
    /// New department name.
    /// </summary>
    /// <example>Department of Artificial Intelligence</example>
    public string Name { get; init; } = null!;

    /// <summary>
    /// New department code (optional).
    /// </summary>
    /// <example>AI</example>
    public string? Code { get; init; }
}