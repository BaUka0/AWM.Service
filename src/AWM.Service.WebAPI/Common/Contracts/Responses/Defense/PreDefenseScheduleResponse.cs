namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for a pre-defense schedule slot.
/// </summary>
public sealed record PreDefenseScheduleResponse
{
    /// <summary>Schedule ID.</summary>
    /// <example>10</example>
    public long Id { get; init; }

    /// <summary>Commission ID.</summary>
    /// <example>3</example>
    public int CommissionId { get; init; }

    /// <summary>StudentWork ID assigned to this slot.</summary>
    /// <example>42</example>
    public long WorkId { get; init; }

    /// <summary>Date and time of the pre-defense.</summary>
    /// <example>2024-04-15T10:00:00Z</example>
    public DateTime DefenseDate { get; init; }

    /// <summary>Physical or virtual location.</summary>
    /// <example>Ауд. 301-А</example>
    public string? Location { get; init; }

    /// <summary>Current average score from all submitted grades. Null if no grades.</summary>
    /// <example>78.5</example>
    public decimal? AverageScore { get; init; }

    /// <summary>Number of grades submitted for this slot.</summary>
    /// <example>3</example>
    public int GradeCount { get; init; }

    /// <summary>Date the schedule was created.</summary>
    public DateTime CreatedAt { get; init; }
}
