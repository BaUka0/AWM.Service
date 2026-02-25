namespace AWM.Service.Application.Features.Defense.Evaluation.Queries.GetEvaluationCriteria;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve evaluation criteria by work type and optional department.
/// </summary>
public sealed record GetEvaluationCriteriaQuery : IRequest<Result<IReadOnlyList<EvaluationCriteriaDto>>>
{
    /// <summary>
    /// Work type ID to get criteria for.
    /// </summary>
    public int WorkTypeId { get; init; }

    /// <summary>
    /// Optional department ID. If null, returns university-wide criteria.
    /// </summary>
    public int? DepartmentId { get; init; }
}
