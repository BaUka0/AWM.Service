namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for a commission member to submit a pre-defense grade.
/// </summary>
public sealed record SubmitPreDefenseGradeRequest
{
    /// <summary>
    /// Commission member ID who is submitting the grade.
    /// </summary>
    /// <example>5</example>
    public int MemberId { get; init; }

    /// <summary>
    /// Evaluation criteria ID to grade.
    /// </summary>
    /// <example>2</example>
    public int CriteriaId { get; init; }

    /// <summary>
    /// Score (0–100).
    /// </summary>
    /// <example>85</example>
    public int Score { get; init; }

    /// <summary>
    /// Optional comment from the commission member.
    /// </summary>
    /// <example>Хорошая защита. Студент уверенно отвечал на вопросы.</example>
    public string? Comment { get; init; }
}
