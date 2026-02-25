namespace AWM.Service.Application.Features.Defense.Evaluation.Queries.GetEvaluationCriteria;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving evaluation criteria.
/// </summary>
public sealed class GetEvaluationCriteriaQueryHandler
    : IRequestHandler<GetEvaluationCriteriaQuery, Result<IReadOnlyList<EvaluationCriteriaDto>>>
{
    private readonly IEvaluationCriteriaRepository _criteriaRepository;

    public GetEvaluationCriteriaQueryHandler(IEvaluationCriteriaRepository criteriaRepository)
    {
        _criteriaRepository = criteriaRepository ?? throw new ArgumentNullException(nameof(criteriaRepository));
    }

    public async Task<Result<IReadOnlyList<EvaluationCriteriaDto>>> Handle(
        GetEvaluationCriteriaQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var criteria = await _criteriaRepository.GetByWorkTypeAsync(
                request.WorkTypeId, request.DepartmentId, cancellationToken);

            var dtos = criteria
                .Where(c => !c.IsDeleted)
                .Select(c => new EvaluationCriteriaDto
                {
                    Id = c.Id,
                    WorkTypeId = c.WorkTypeId,
                    DepartmentId = c.DepartmentId,
                    CriteriaName = c.CriteriaName,
                    MaxScore = c.MaxScore,
                    Weight = c.Weight
                })
                .ToList();

            return Result.Success<IReadOnlyList<EvaluationCriteriaDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<EvaluationCriteriaDto>>(
                new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}
