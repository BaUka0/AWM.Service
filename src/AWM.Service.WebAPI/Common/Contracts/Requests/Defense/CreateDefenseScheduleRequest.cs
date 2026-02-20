namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for creating a defense schedule slot.
/// </summary>
public sealed record CreateDefenseScheduleRequest
{
    /// <summary>
    /// Commission ID (must be of type GAK).
    /// </summary>
    /// <example>5</example>
    public int CommissionId { get; init; }

    /// <summary>
    /// Date and time of the defense.
    /// </summary>
    /// <example>2024-06-20T09:00:00Z</example>
    public DateTime DefenseDate { get; init; }

    /// <summary>
    /// Physical or virtual location. Optional.
    /// </summary>
    /// <example>Ауд. 405-Б</example>
    public string? Location { get; init; }
}
