namespace AWM.Service.Application.Features.Common.Stages.Commands.CreateStage;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class CreateStageCommandHandler : IRequestHandler<CreateStageCommand, Result<int>>
{
    private readonly IStageRepository _stageRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateStageCommandHandler> _logger;

    public CreateStageCommandHandler(
        IStageRepository stageRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<CreateStageCommandHandler> logger)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(CreateStageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("Attempting to create stage for Dept={DeptId}, Year={YearId}, Stage={Stage} by User={UserId}",
                request.DepartmentId, request.SemesterId, request.WorkflowStageId, userId);

            // Check for overlapping stages of the same stage in the same department/year
            var existingStages = await _stageRepository.GetByDepartmentAsync(request.DepartmentId, request.SemesterId, cancellationToken);
            var overlapping = existingStages
                .Where(p => !p.IsDeleted && p.WorkflowStageId == request.WorkflowStageId)
                .Any(p => request.StartDate < p.EndDate && request.EndDate > p.StartDate);

            if (overlapping)
            {
                _logger.LogWarning("CreateStage failed: Overlapping stage for Stage={Stage} in Dept={DeptId}, Year={YearId}",
                    request.WorkflowStageId, request.DepartmentId, request.SemesterId);
                return Result.Failure<int>(new Error("409", "An overlapping stage for this workflow stage already exists."));
            }

            if (!userId.HasValue)
            {
                _logger.LogWarning("CreateStage failed: User ID is not available.");
                return Result.Failure<int>(new Error("401", "User ID is not available."));
            }

            var stage = new Stage(
                request.DepartmentId,
                request.SemesterId,
                request.WorkflowStageId,
                request.StartDate,
                request.EndDate,
                userId.Value);

            await _stageRepository.AddAsync(stage, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created stage with ID={StageId} for Dept={DeptId}", stage.Id, request.DepartmentId);
            return Result.Success(stage.Id);
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "CreateStage validation failed: {Message}", argEx.Message);
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateStage failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure<int>(new Error("500", $"An error occurred while creating the Stage: {ex.Message}"));
        }
    }
}
