namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for a grade submitted by a commission member.
/// </summary>
public sealed record GradeResponse
{
    /// <summary>Grade ID.</summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>Schedule ID this grade belongs to.</summary>
    /// <example>15</example>
    public long ScheduleId { get; init; }

    /// <summary>Commission member ID who submitted the grade.</summary>
    /// <example>7</example>
    public int MemberId { get; init; }

    /// <summary>Evaluation criteria ID that was graded.</summary>
    /// <example>3</example>
    public int CriteriaId { get; init; }

    /// <summary>Score value (0–100).</summary>
    /// <example>90</example>
    public int Score { get; init; }

    /// <summary>Optional comment from the commission member.</summary>
    /// <example>Отличная работа.</example>
    public string? Comment { get; init; }

    /// <summary>Date and time the grade was submitted.</summary>
    public DateTime GradedAt { get; init; }
}
