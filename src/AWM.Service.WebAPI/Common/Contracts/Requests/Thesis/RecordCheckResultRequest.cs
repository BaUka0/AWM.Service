namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for an expert recording a quality check result.
/// The check type is inferred from the existing pending record identified by {checkId} in the route.
/// </summary>
public sealed record RecordCheckResultRequest
{
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
