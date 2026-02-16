namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

/// <summary>
/// Request contract for creating a degree level.
/// </summary>
public sealed record CreateDegreeLevelRequest
{
    /// <summary>
    /// Degree level name (e.g. "Bachelor", "Master", "PhD").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Duration in years (e.g. 4 for Bachelor, 2 for Master, 3 for PhD).
    /// </summary>
    public int DurationYears { get; init; }
}