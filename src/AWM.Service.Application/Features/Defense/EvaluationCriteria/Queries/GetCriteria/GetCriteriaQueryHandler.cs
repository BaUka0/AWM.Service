using AWM.Service.Application.Features.Defense.EvaluationCriteria.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Queries.GetCriteria;

public sealed class GetCriteriaQueryHandler : IRequestHandler<GetCriteriaQuery, Result<IReadOnlyList<EvaluationCriteriaDto>>>
{
    private readonly IEvaluationCriteriaRepository _criteriaRepository;

    public GetCriteriaQueryHandler(IEvaluationCriteriaRepository criteriaRepository)
    {
        _criteriaRepository = criteriaRepository;
    }

    public async Task<Result<IReadOnlyList<EvaluationCriteriaDto>>> Handle(GetCriteriaQuery request, CancellationToken cancellationToken)
    {
        var criteria = await _criteriaRepository.GetByWorkTypeAsync(
            request.WorkTypeId,
            request.OrgUnitId,
            request.SpecialityId,
            cancellationToken);

        var filtered = criteria.AsEnumerable();
        if (request.DefenseStageType.HasValue)
        {
            filtered = filtered.Where(c => c.DefenseStageType == request.DefenseStageType.Value);
        }

        var response = filtered
            .OrderBy(c => c.SortOrder)
            .Select(c => new EvaluationCriteriaDto(
                c.Id,
                c.WorkTypeId,
                c.CriteriaName,
                c.MaxScore,
                c.Weight,
                c.OrgUnitId,
                c.SpecialityId,
                c.DefenseStageType,
                c.SortOrder)).ToList();

        return Result.Success<IReadOnlyList<EvaluationCriteriaDto>>(response);
    }
}
