namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for finalizing a pre-defense attempt.
/// </summary>
public sealed record FinalizePreDefenseRequest
{
    /// <summary>
    /// Final average score to record.
    /// </summary>
    /// <example>85.5</example>
    public decimal AverageScore { get; init; }

    /// <summary>
    /// Whether the student passed the pre-defense.
    /// </summary>
    /// <example>true</example>
    public bool IsPassed { get; init; }
}
