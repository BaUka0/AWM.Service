namespace AWM.Service.WebAPI.Common.Contracts.Requests.Institutes;

/// <summary>
/// Request model for updating an institute.
/// </summary>
public record UpdateInstituteRequest
{
    /// <summary>
    /// New name for the institute.
    /// </summary>
    public string Name { get; init; } = null!;
}
