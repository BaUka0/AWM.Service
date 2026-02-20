namespace AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetChecksByWork;

using AWM.Service.Application.Features.Thesis.QualityChecks.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve the full history of quality checks for a specific work.
/// </summary>
public sealed record GetChecksByWorkQuery : IRequest<Result<IReadOnlyList<QualityCheckDto>>>
{
    /// <summary>
    /// StudentWork ID to retrieve checks for.
    /// </summary>
    public long WorkId { get; init; }
}
