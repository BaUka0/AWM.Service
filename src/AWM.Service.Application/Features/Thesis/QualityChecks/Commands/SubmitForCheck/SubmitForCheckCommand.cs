namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;

using AWM.Service.Domain.Thesis.Enums;
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
    /// Type of check to perform.
    /// </summary>
    public CheckType CheckType { get; init; }

    /// <summary>
    /// Optional comment from the student.
    /// </summary>
    public string? Comment { get; init; }
}
