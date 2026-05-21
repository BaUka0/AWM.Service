namespace AWM.Service.Application.Features.Common.Stages.Commands.UpdateStage;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class UpdateStageCommandHandler : IRequestHandler<UpdateStageCommand, Result>
{
    private readonly IStageRepository _stageRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateStageCommandHandler> _logger;

    public UpdateStageCommandHandler(
        IStageRepository stageRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<UpdateStageCommandHandler> logger)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(UpdateStageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("Attempting to update stage ID={StageId} by User={UserId}", request.StageId, userId);

            var stage = await _stageRepository.GetByIdAsync(request.StageId, cancellationToken);
            if (stage is null || stage.IsDeleted)
            {
                _logger.LogWarning("UpdateStage failed: Stage ID={StageId} not found.", request.StageId);
                return Result.Failure(new Error("404", $"Stage with ID {request.StageId} not found."));
            }

            if (!userId.HasValue)
            {
                _logger.LogWarning("UpdateStage failed: User ID is not available.");
                return Result.Failure(new Error("401", "User ID is not available."));
            }

            // Check for overlaps if dates are being changed
            if (request.StartDate.HasValue || request.EndDate.HasValue)
            {
                var newStartDate = request.StartDate ?? stage.StartDate;
                var newEndDate = request.EndDate ?? stage.EndDate;

                var existingStages = await _stageRepository.GetByDepartmentAsync(stage.OrgUnitId, stage.SemesterId, cancellationToken);
                var overlapping = existingStages
                    .Where(p => !p.IsDeleted && p.WorkflowStageId == stage.WorkflowStageId && p.Id != stage.Id)
                    .Any(p => newStartDate < p.EndDate && newEndDate > p.StartDate);

                if (overlapping)
                {
                    _logger.LogWarning("UpdateStage failed: Overlapping stage for Stage={Stage} in Dept={DeptId}, Year={YearId}",
                        stage.WorkflowStageId, stage.OrgUnitId, stage.SemesterId);
                    return Result.Failure(new Error("409", "An overlapping stage for this workflow stage already exists."));
                }

                stage.UpdateDates(newStartDate, newEndDate, userId.Value);
            }

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                    stage.Activate(userId.Value);
                else
                    stage.Deactivate(userId.Value);
            }

            await _stageRepository.UpdateAsync(stage, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated stage ID={StageId}", request.StageId);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "UpdateStage validation failed for ID={StageId}: {Message}", request.StageId, argEx.Message);
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStage failed for ID={StageId}", request.StageId);
            return Result.Failure(new Error("500", $"An error occurred while updating the Stage: {ex.Message}"));
        }
    }
}
