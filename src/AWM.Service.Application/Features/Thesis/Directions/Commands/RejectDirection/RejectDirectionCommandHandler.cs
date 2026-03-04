namespace AWM.Service.Application.Features.Thesis.Directions.Commands.RejectDirection;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for rejecting a direction.
/// </summary>
public sealed class RejectDirectionCommandHandler
    : IRequestHandler<RejectDirectionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectDirectionCommandHandler> _logger;

    public RejectDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<RejectDirectionCommandHandler> logger)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        RejectDirectionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to reject direction ID={DirectionId} by User={UserId}", request.Id, userId);

        // Get existing direction (tracked)
        var direction = await _directionRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (direction is null)
        {
            _logger.LogWarning("RejectDirection failed: Direction ID={DirectionId} not found.", request.Id);
            return Result.Failure(new Error(
                "404",
                $"Direction with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (direction.IsDeleted)
        {
            _logger.LogWarning("RejectDirection failed: Direction ID={DirectionId} is deleted.", request.Id);
            return Result.Failure(new Error(
                "409",
                $"Direction with ID {request.Id} has been deleted."));
        }

        // Get Submitted state to verify current state
        var submittedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Submitted", cancellationToken);

        if (submittedState is null)
        {
            _logger.LogError("RejectDirection failed: Submitted state not found for WorkType ID={WorkTypeId}", direction.WorkTypeId);
            return Result.Failure(new Error(
                "404",
                "Submitted state not found for this work type."));
        }

        // Verify direction is in submitted state
        if (direction.CurrentStateId != submittedState.Id)
        {
            _logger.LogWarning("RejectDirection failed: Direction ID={DirectionId} is in state {StateId}, not Submitted.",
                request.Id, direction.CurrentStateId);
            return Result.Failure(new Error(
                "409",
                "Only submitted directions can be rejected. Current state does not allow rejection."));
        }

        // Get Rejected state
        var rejectedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Rejected", cancellationToken);

        if (rejectedState is null)
        {
            _logger.LogError("RejectDirection failed: Rejected state not found for WorkType ID={WorkTypeId}", direction.WorkTypeId);
            return Result.Failure(new Error(
                "404",
                "Rejected state not found for this work type."));
        }

        if (!userId.HasValue)
        {
            _logger.LogWarning("RejectDirection failed: User ID is not available.");
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        try
        {
            // Reject using domain method (raises DirectionRejectedEvent)
            direction.Reject(rejectedState.Id, userId.Value, request.Comment);

            // Save changes
            await _directionRepository.UpdateAsync(direction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully rejected direction ID={DirectionId} by User={UserId}", request.Id, userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RejectDirection failed for ID={DirectionId}", request.Id);
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}