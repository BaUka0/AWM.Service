namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

/// <summary>
/// Request payload for updating a degree level.
/// </summary>
public sealed record UpdateDegreeLevelRequest
{
    public string Name { get; init; } = string.Empty;
    public int DurationYears { get; init; }
}
