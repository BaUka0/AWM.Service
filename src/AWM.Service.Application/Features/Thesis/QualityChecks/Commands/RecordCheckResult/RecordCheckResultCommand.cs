namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.RecordCheckResult;

using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for an expert recording the result of a quality check.
/// </summary>
public sealed record RecordCheckResultCommand : IRequest<Result<long>>
{
    /// <summary>
    /// StudentWork ID to record the check result for.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// Type of check being recorded.
    /// </summary>
    public CheckType CheckType { get; init; }

    /// <summary>
    /// Whether the work passed this check.
    /// </summary>
    public bool IsPassed { get; init; }

    /// <summary>
    /// Numeric result value (e.g. plagiarism percentage for AntiPlagiarism checks).
    /// </summary>
    public decimal? ResultValue { get; init; }

    /// <summary>
    /// Expert's comment on the check result.
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    /// Path to the supporting check document (e.g. anti-plagiarism report).
    /// </summary>
    public string? DocumentPath { get; init; }
}
