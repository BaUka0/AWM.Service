namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for a defense slot with detailed grade information.
/// </summary>
public sealed record DefenseSlotResponse
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

    /// <summary>Individual grades from commission members.</summary>
    public IReadOnlyList<GradeResponse> Grades { get; init; } = Array.Empty<GradeResponse>();

    /// <summary>Date the schedule was created.</summary>
    public DateTime CreatedAt { get; init; }
}
