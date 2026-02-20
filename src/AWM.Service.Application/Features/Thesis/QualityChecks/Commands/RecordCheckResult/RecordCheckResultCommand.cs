namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.RecordCheckResult;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for an expert recording the result of a quality check.
/// The CheckId identifies the pending QualityCheck created by SubmitForCheck.
/// </summary>
public sealed record RecordCheckResultCommand : IRequest<Result<long>>
{
    /// <summary>
    /// StudentWork ID that owns the quality check.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// ID of the pending QualityCheck (returned by SubmitForCheck) to complete.
    /// </summary>
    public long CheckId { get; init; }

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
