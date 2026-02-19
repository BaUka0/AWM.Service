namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Request contract for an expert recording a quality check result.
/// </summary>
public sealed record RecordCheckResultRequest
{
    /// <summary>
    /// Type of check being recorded (0 = NormControl, 1 = SoftwareCheck, 2 = AntiPlagiarism).
    /// </summary>
    /// <example>2</example>
    public CheckType CheckType { get; init; }

    /// <summary>
    /// Whether the work passed this check.
    /// </summary>
    /// <example>true</example>
    public bool IsPassed { get; init; }

    /// <summary>
    /// Numeric result value (e.g. plagiarism percentage for AntiPlagiarism). Optional.
    /// </summary>
    /// <example>12.5</example>
    public decimal? ResultValue { get; init; }

    /// <summary>
    /// Expert's comment on the result.
    /// </summary>
    /// <example>Работа прошла проверку. Процент оригинальности: 87.5%</example>
    public string? Comment { get; init; }

    /// <summary>
    /// Path to the supporting check document. Optional.
    /// </summary>
    /// <example>/documents/antiplag-report-2024-001.pdf</example>
    public string? DocumentPath { get; init; }
}
