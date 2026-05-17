namespace AWM.Service.WebAPI.Common.Contracts.Requests.Org;

/// <summary>
/// Request contract for creating a new institute.
/// </summary>
public sealed record CreateInstituteRequest
{
    /// <summary>
    /// Institute name.
    /// </summary>
    /// <example>Faculty of Computer Science</example>
    public string Name { get; init; } = null!;
}