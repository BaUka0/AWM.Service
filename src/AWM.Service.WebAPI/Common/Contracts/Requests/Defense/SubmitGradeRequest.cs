namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for a GAK commission member to submit a defense grade.
/// </summary>
public sealed record SubmitGradeRequest
{
    /// <summary>
    /// Assignment record ID (StaffAssignment) for the member submitting the grade.
    /// </summary>
    /// <example>7</example>
    public long AssignmentId { get; init; }

    /// <summary>
    /// Evaluation criteria ID to grade.
    /// </summary>
    /// <example>3</example>
    public int CriteriaId { get; init; }

    /// <summary>
    /// Score (0–100).
    /// </summary>
    /// <example>90</example>
    public int Score { get; init; }

    /// <summary>
    /// Optional comment from the commission member.
    /// </summary>
    /// <example>Отличная работа. Студент показал глубокие знания.</example>
    public string? Comment { get; init; }
}
