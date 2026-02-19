namespace AWM.Service.Application.Features.Thesis.QualityChecks.DTOs;

using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Data Transfer Object for a quality check record.
/// </summary>
public sealed record QualityCheckDto
{
    /// <summary>
    /// Quality check record ID.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Work ID this check belongs to.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// Type of check (NormControl, SoftwareCheck, AntiPlagiarism).
    /// </summary>
    public string CheckType { get; init; } = null!;

    /// <summary>
    /// Attempt number for this check type on this work.
    /// </summary>
    public int AttemptNumber { get; init; }

    /// <summary>
    /// Whether the check was passed.
    /// </summary>
    public bool IsPassed { get; init; }

    /// <summary>
    /// Numeric result (e.g. plagiarism percentage). Null if not applicable.
    /// </summary>
    public decimal? ResultValue { get; init; }

    /// <summary>
    /// Expert's comment on the result.
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    /// Path to supporting document (e.g. anti-plagiarism report).
    /// </summary>
    public string? DocumentPath { get; init; }

    /// <summary>
    /// ID of the expert who performed the check. Null if pending.
    /// </summary>
    public int? AssignedExpertId { get; init; }

    /// <summary>
    /// Date and time when the check was recorded.
    /// </summary>
    public DateTime CheckedAt { get; init; }
}
