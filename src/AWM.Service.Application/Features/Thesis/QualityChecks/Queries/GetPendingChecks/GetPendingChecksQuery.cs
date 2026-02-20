namespace AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetPendingChecks;

using AWM.Service.Application.Features.Thesis.QualityChecks.DTOs;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve quality checks that are pending expert review (AssignedExpertId is null).
/// Intended for the expert's work queue.
/// </summary>
public sealed record GetPendingChecksQuery : IRequest<Result<IReadOnlyList<QualityCheckDto>>>
{
    /// <summary>
    /// Department ID to filter pending checks.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID to filter pending checks.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Optional check type filter. If null, returns all types.
    /// </summary>
    public CheckType? CheckType { get; init; }
}
