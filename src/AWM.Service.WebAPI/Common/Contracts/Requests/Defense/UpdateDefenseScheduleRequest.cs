namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for updating (rescheduling) a defense schedule slot.
/// </summary>
public sealed record UpdateDefenseScheduleRequest
{
    /// <summary>
    /// New defense date. Optional — if null, date is not changed.
    /// </summary>
    /// <example>2024-06-25T14:00:00Z</example>
    public DateTime? DefenseDate { get; init; }

    /// <summary>
    /// New location. Optional — if null, location is not changed.
    /// </summary>
    /// <example>Ауд. 301-А</example>
    public string? Location { get; init; }
}
