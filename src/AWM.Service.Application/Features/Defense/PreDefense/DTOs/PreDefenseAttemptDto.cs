namespace AWM.Service.Application.Features.Defense.PreDefense.DTOs;

using AWM.Service.Domain.Defense.Enums;

/// <summary>
/// DTO representing a single pre-defense attempt for a student work.
/// </summary>
public sealed record PreDefenseAttemptDto
{
    /// <summary>
    /// Attempt ID.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// StudentWork ID.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// Pre-defense sequence number (1, 2, or 3).
    /// </summary>
    public int PreDefenseNumber { get; init; }

    /// <summary>
    /// Optional schedule ID this attempt is linked to.
    /// </summary>
    public long? ScheduleId { get; init; }

    /// <summary>
    /// Attendance status (Attended / Absent / Excused).
    /// </summary>
    public string AttendanceStatus { get; init; } = null!;

    /// <summary>
    /// Average score given by the commission. Null if not yet graded.
    /// </summary>
    public decimal? AverageScore { get; init; }

    /// <summary>
    /// Whether the attempt was passed.
    /// </summary>
    public bool IsPassed { get; init; }

    /// <summary>
    /// Whether the student needs to retake (not passed and number < 3).
    /// </summary>
    public bool NeedsRetake { get; init; }

    /// <summary>
    /// Date and time of the attempt.
    /// </summary>
    public DateTime AttemptDate { get; init; }

    /// <summary>
    /// Date the attempt record was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
