namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for scheduling a work for a pre-defense slot.
/// </summary>
public sealed record SchedulePreDefenseRequest
{
    /// <summary>
    /// Commission ID (must be of type PreDefense).
    /// </summary>
    /// <example>3</example>
    public int CommissionId { get; init; }

    /// <summary>
    /// Date and time of the pre-defense.
    /// </summary>
    /// <example>2024-04-15T10:00:00Z</example>
    public DateTime DefenseDate { get; init; }

    /// <summary>
    /// Physical or virtual location. Optional.
    /// </summary>
    /// <example>Ауд. 301-А</example>
    public string? Location { get; init; }
}
