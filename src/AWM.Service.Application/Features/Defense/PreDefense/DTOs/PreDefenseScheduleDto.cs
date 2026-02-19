namespace AWM.Service.Application.Features.Defense.PreDefense.DTOs;

/// <summary>
/// DTO representing a scheduled pre-defense slot with grades.
/// </summary>
public sealed record PreDefenseScheduleDto
{
    /// <summary>
    /// Schedule ID.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Commission ID this slot belongs to.
    /// </summary>
    public int CommissionId { get; init; }

    /// <summary>
    /// StudentWork ID scheduled for this slot.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// DateTime of the pre-defense.
    /// </summary>
    public DateTime DefenseDate { get; init; }

    /// <summary>
    /// Physical or virtual location.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Current average score calculated from all grades. Null if no grades yet.
    /// </summary>
    public decimal? AverageScore { get; init; }

    /// <summary>
    /// Number of grades already submitted.
    /// </summary>
    public int GradeCount { get; init; }

    /// <summary>
    /// Date the schedule was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
