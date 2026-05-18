namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command representing a student submitting their work for a quality check.
/// Creates a pending quality check record with no result yet.
/// </summary>
public sealed record SubmitForCheckCommand : IRequest<Result<long>>
{
    /// <summary>
    /// StudentWork ID to submit for check.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// Check type ID (FK to Thesis.CheckTypes). 1=NormControl, 2=SoftwareCheck, 3=AntiPlagiarism.
    /// </summary>
    public int CheckTypeId { get; init; }

    /// <summary>
    /// Optional comment from the student.
    /// </summary>
    public string? Comment { get; init; }
}
