namespace AWM.Service.WebAPI.Common.Contracts.Requests.Institutes;

/// <summary>
/// Request model for creating an institute.
/// </summary>
public record CreateInstituteRequest
{
    /// <summary>
    /// Name of the institute.
    /// </summary>
    public string Name { get; init; } = null!;
}
