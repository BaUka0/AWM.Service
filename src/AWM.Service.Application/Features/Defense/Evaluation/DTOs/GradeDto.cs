namespace AWM.Service.Application.Features.Defense.Evaluation.DTOs;

/// <summary>
/// DTO representing an individual grade from a commission member.
/// </summary>
public sealed record GradeDto
{
    /// <summary>Grade ID.</summary>
    public long Id { get; init; }

    /// <summary>Schedule ID this grade belongs to.</summary>
    public long ScheduleId { get; init; }

    /// <summary>Commission member ID who submitted the grade.</summary>
    public int MemberId { get; init; }

    /// <summary>Evaluation criteria ID that was graded.</summary>
    public int CriteriaId { get; init; }

    /// <summary>Score value.</summary>
    public int Score { get; init; }

    /// <summary>Optional comment from the commission member.</summary>
    public string? Comment { get; init; }

    /// <summary>Date and time the grade was submitted.</summary>
    public DateTime GradedAt { get; init; }
}
