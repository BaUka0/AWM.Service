namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for a pre-defense attempt record.
/// </summary>
public sealed record PreDefenseAttemptResponse
{
    /// <summary>Attempt ID.</summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>StudentWork ID.</summary>
    /// <example>42</example>
    public long WorkId { get; init; }

    /// <summary>Pre-defense sequence number (1, 2, or 3).</summary>
    /// <example>1</example>
    public int PreDefenseNumber { get; init; }

    /// <summary>Optional schedule ID the attempt is linked to.</summary>
    /// <example>10</example>
    public long? ScheduleId { get; init; }

    /// <summary>Attendance status (Attended / Absent / Excused).</summary>
    /// <example>Attended</example>
    public string AttendanceStatus { get; init; } = null!;

    /// <summary>Average score from the commission. Null if not yet graded.</summary>
    /// <example>78.5</example>
    public decimal? AverageScore { get; init; }

    /// <summary>Whether the attempt was passed.</summary>
    /// <example>true</example>
    public bool IsPassed { get; init; }

    /// <summary>Whether the student needs to retake.</summary>
    /// <example>false</example>
    public bool NeedsRetake { get; init; }

    /// <summary>Date and time of the attempt.</summary>
    /// <example>2024-04-15T10:00:00Z</example>
    public DateTime AttemptDate { get; init; }

    /// <summary>Date the record was created.</summary>
    public DateTime CreatedAt { get; init; }
}
