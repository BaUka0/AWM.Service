namespace AWM.Service.WebAPI.Common.Contracts.Requests.Institutes;

/// <summary>
/// Request model for creating an institute.
/// </summary>
public record CreateInstituteRequest
{
    /// <summary>
    /// University ID to which the institute belongs.
    /// </summary>
    public int UniversityId { get; init; }

    /// <summary>
    /// Name of the institute.
    /// </summary>
    public string Name { get; init; } = null!;
}
