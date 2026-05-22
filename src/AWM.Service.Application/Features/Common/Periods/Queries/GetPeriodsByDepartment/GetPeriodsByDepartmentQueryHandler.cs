namespace AWM.Service.Application.Features.Common.Stages.Queries.GetStagesByDepartment;

using AWM.Service.Application.Features.Common.Stages.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetStagesByDepartmentQueryHandler : IRequestHandler<GetStagesByDepartmentQuery, Result<IReadOnlyList<StageDto>>>
{
    private readonly IStageRepository _stageRepository;

    public GetStagesByDepartmentQueryHandler(IStageRepository stageRepository)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
    }

    public async Task<Result<IReadOnlyList<StageDto>>> Handle(GetStagesByDepartmentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stages = await _stageRepository.GetByDepartmentAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

            var dtos = stages
                .Where(p => !p.IsDeleted)
                .Select(p => new StageDto
                {
                    Id = p.Id,
                    OrgUnitId = p.OrgUnitId,
                    SpecialityId = p.SpecialityId,
                    SemesterId = p.SemesterId,
                    WorkflowStageId = p.WorkflowStageId,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive,
                    IsCurrentlyOpen = p.IsCurrentlyOpen(),
                    CreatedAt = p.CreatedAt,
                    CreatedBy = p.CreatedBy,
                    LastModifiedAt = p.LastModifiedAt,
                    LastModifiedBy = p.LastModifiedBy
                })
                .ToList();

            return Result.Success<IReadOnlyList<StageDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<StageDto>>(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
