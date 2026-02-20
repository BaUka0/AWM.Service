namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for a defense schedule slot.
/// </summary>
public sealed record ScheduleResponse
{
    /// <summary>Schedule ID.</summary>
    /// <example>15</example>
    public long Id { get; init; }

    /// <summary>Commission ID.</summary>
    /// <example>5</example>
    public int CommissionId { get; init; }

    /// <summary>StudentWork ID assigned to this slot.</summary>
    /// <example>42</example>
    public long WorkId { get; init; }

    /// <summary>Date and time of the defense.</summary>
    /// <example>2024-06-20T09:00:00Z</example>
    public DateTime DefenseDate { get; init; }

    /// <summary>Physical or virtual location.</summary>
    /// <example>Ауд. 405-Б</example>
    public string? Location { get; init; }

    /// <summary>Current average score from all submitted grades. Null if no grades.</summary>
    /// <example>85.5</example>
    public decimal? AverageScore { get; init; }

    /// <summary>Number of grades submitted for this slot.</summary>
    /// <example>4</example>
    public int GradeCount { get; init; }

    /// <summary>Date the schedule was created.</summary>
    public DateTime CreatedAt { get; init; }
}
