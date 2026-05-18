namespace AWM.Service.Application.Features.Common.Stages.Queries.GetActiveStage;

using AWM.Service.Application.Features.Common.Stages.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetActiveStageQueryHandler : IRequestHandler<GetActiveStageQuery, Result<StageDto?>>
{
    private readonly IStageRepository _stageRepository;

    public GetActiveStageQueryHandler(IStageRepository stageRepository)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
    }

    public async Task<Result<StageDto?>> Handle(GetActiveStageQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stage = request.WorkflowStageId.HasValue
                ? await _stageRepository.GetActiveByStageAsync(
                    request.DepartmentId,
                    request.SemesterId,
                    request.WorkflowStageId.Value,
                    cancellationToken)
                : await _stageRepository.GetActiveStageAsync(
                    request.DepartmentId,
                    request.SemesterId,
                    cancellationToken);

            if (stage is null || stage.IsDeleted)
                return Result.Success<StageDto?>(null);

            var dto = new StageDto
            {
                Id = stage.Id,
                DepartmentId = stage.DepartmentId,
                SemesterId = stage.SemesterId,
                WorkflowStageId = stage.WorkflowStageId,
                StartDate = stage.StartDate,
                EndDate = stage.EndDate,
                IsActive = stage.IsActive,
                IsCurrentlyOpen = stage.IsCurrentlyOpen(),
                CreatedAt = stage.CreatedAt,
                CreatedBy = stage.CreatedBy,
                LastModifiedAt = stage.LastModifiedAt,
                LastModifiedBy = stage.LastModifiedBy
            };

            return Result.Success<StageDto?>(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<StageDto?>(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
