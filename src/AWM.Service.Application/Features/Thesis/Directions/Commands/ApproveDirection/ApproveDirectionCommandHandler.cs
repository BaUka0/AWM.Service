namespace AWM.Service.Application.Features.Thesis.Directions.Commands.ApproveDirection;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for approving a direction.
/// </summary>
public sealed class ApproveDirectionCommandHandler
    : IRequestHandler<ApproveDirectionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveDirectionCommandHandler> _logger;

    public ApproveDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<ApproveDirectionCommandHandler> logger)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ApproveDirectionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to approve direction ID={DirectionId} by User={UserId}", request.Id, userId);

        // Get existing direction
        var direction = await _directionRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (direction is null)
        {
            _logger.LogWarning("ApproveDirection failed: Direction ID={DirectionId} not found.", request.Id);
            return Result.Failure(new Error(
                "404",
                $"Direction with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (direction.IsDeleted)
        {
            _logger.LogWarning("ApproveDirection failed: Direction ID={DirectionId} is deleted.", request.Id);
            return Result.Failure(new Error(
                "409",
                $"Direction with ID {request.Id} has been deleted."));
        }

        // Get Submitted state to verify current state
        var submittedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Submitted", cancellationToken);

        if (submittedState is null)
        {
            _logger.LogError("ApproveDirection failed: Submitted state not found for WorkType ID={WorkTypeId}", direction.WorkTypeId);
            return Result.Failure(new Error(
                "404",
                "Submitted state not found for this work type."));
        }

        // Verify direction is in submitted state
        if (direction.CurrentStateId != submittedState.Id)
        {
            _logger.LogWarning("ApproveDirection failed: Direction ID={DirectionId} is in state {StateId}, not Submitted.",
                request.Id, direction.CurrentStateId);
            return Result.Failure(new Error(
                "409",
                "Only submitted directions can be approved. Current state does not allow approval."));
        }

        // Get Approved state
        var approvedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Approved", cancellationToken);

        if (approvedState is null)
        {
            _logger.LogError("ApproveDirection failed: Approved state not found for WorkType ID={WorkTypeId}", direction.WorkTypeId);
            return Result.Failure(new Error(
                "404",
                "Approved state not found for this work type."));
        }

        if (!userId.HasValue)
        {
            _logger.LogWarning("ApproveDirection failed: User ID is not available.");
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        try
        {
            // Approve using domain method (raises DirectionApprovedEvent)
            direction.Approve(approvedState.Id, userId.Value);

            // Save changes
            await _directionRepository.UpdateAsync(direction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully approved direction ID={DirectionId} by User={UserId}", request.Id, userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ApproveDirection failed for ID={DirectionId}", request.Id);
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}