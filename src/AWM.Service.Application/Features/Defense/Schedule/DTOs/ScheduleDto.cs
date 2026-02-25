namespace AWM.Service.Application.Features.Defense.Schedule.DTOs;

/// <summary>
/// DTO representing a defense schedule slot.
/// </summary>
public sealed record ScheduleDto
{
    /// <summary>Schedule ID.</summary>
    public long Id { get; init; }

    /// <summary>Commission ID this slot belongs to.</summary>
    public int CommissionId { get; init; }

    /// <summary>StudentWork ID assigned to this slot.</summary>
    public long WorkId { get; init; }

    /// <summary>Date and time of the defense.</summary>
    public DateTime DefenseDate { get; init; }

    /// <summary>Physical or virtual location.</summary>
    public string? Location { get; init; }

    /// <summary>Current average score from all submitted grades. Null if no grades.</summary>
    public decimal? AverageScore { get; init; }

    /// <summary>Number of grades submitted for this slot.</summary>
    public int GradeCount { get; init; }

    /// <summary>Date the schedule was created.</summary>
    public DateTime CreatedAt { get; init; }
}
