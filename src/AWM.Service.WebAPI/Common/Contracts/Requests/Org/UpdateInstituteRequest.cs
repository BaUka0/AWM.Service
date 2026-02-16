namespace AWM.Service.WebAPI.Common.Contracts.Requests.Org;

/// <summary>
/// Request contract for updating an existing institute.
/// </summary>
public sealed record UpdateInstituteRequest
{
    /// <summary>
    /// New institute name.
    /// </summary>
    /// <example>Faculty of Engineering</example>
    public string Name { get; init; } = null!;
}