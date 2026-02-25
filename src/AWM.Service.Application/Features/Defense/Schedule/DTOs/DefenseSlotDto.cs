namespace AWM.Service.Application.Features.Defense.Schedule.DTOs;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;

/// <summary>
/// DTO representing a single defense slot with detailed grade information.
/// </summary>
public sealed record DefenseSlotDto
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

    /// <summary>Individual grades from commission members.</summary>
    public IReadOnlyList<GradeDto> Grades { get; init; } = Array.Empty<GradeDto>();

    /// <summary>Date the schedule was created.</summary>
    public DateTime CreatedAt { get; init; }
}
