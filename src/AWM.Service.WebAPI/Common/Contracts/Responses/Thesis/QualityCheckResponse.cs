namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for a quality check record.
/// </summary>
public sealed record QualityCheckResponse
{
    /// <summary>
    /// Quality check record ID.
    /// </summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>
    /// Work ID this check belongs to.
    /// </summary>
    /// <example>42</example>
    public long WorkId { get; init; }

    /// <summary>
    /// Type of check (NormControl, SoftwareCheck, AntiPlagiarism).
    /// </summary>
    /// <example>AntiPlagiarism</example>
    public string CheckType { get; init; } = null!;

    /// <summary>
    /// Attempt number for this check type.
    /// </summary>
    /// <example>1</example>
    public int AttemptNumber { get; init; }

    /// <summary>
    /// Whether the check was passed.
    /// </summary>
    /// <example>true</example>
    public bool IsPassed { get; init; }

    /// <summary>
    /// Numeric result (e.g. plagiarism percentage). Null if not applicable.
    /// </summary>
    /// <example>12.5</example>
    public decimal? ResultValue { get; init; }

    /// <summary>
    /// Expert's comment on the result.
    /// </summary>
    /// <example>Работа прошла проверку на антиплагиат.</example>
    public string? Comment { get; init; }

    /// <summary>
    /// Path to supporting document. Null if not provided.
    /// </summary>
    /// <example>/documents/antiplag-report-2024-001.pdf</example>
    public string? DocumentPath { get; init; }

    /// <summary>
    /// ID of the expert who performed the check. Null if pending.
    /// </summary>
    /// <example>7</example>
    public int? AssignedExpertId { get; init; }

    /// <summary>
    /// Date and time when the check was recorded.
    /// </summary>
    /// <example>2024-03-10T14:00:00Z</example>
    public DateTime CheckedAt { get; init; }
}
