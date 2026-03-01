namespace AWM.Service.Application.Features.Thesis.Directions.Commands.SubmitDirection;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for submitting a direction for department review.
/// </summary>
public sealed class SubmitDirectionCommandHandler
    : IRequestHandler<SubmitDirectionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitDirectionCommandHandler> _logger;

    public SubmitDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IStaffRepository staffRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<SubmitDirectionCommandHandler> logger)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _staffRepository = staffRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        SubmitDirectionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to submit direction ID={DirectionId} by User={UserId}", request.Id, userId);

        // Get existing direction
        var direction = await _directionRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (direction is null)
        {
            _logger.LogWarning("SubmitDirection failed: Direction ID={DirectionId} not found.", request.Id);
            return Result.Failure(new Error(
                "404",
                $"Direction with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (direction.IsDeleted)
        {
            _logger.LogWarning("SubmitDirection failed: Direction ID={DirectionId} is deleted.", request.Id);
            return Result.Failure(new Error(
                "409",
                $"Direction with ID {request.Id} has been deleted."));
        }

        if (!userId.HasValue)
        {
            _logger.LogWarning("SubmitDirection failed: User ID is not available.");
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        // Verify user is the supervisor (authorization check)
        var staff = await _staffRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (staff is null || direction.SupervisorId != staff.Id)
        {
            _logger.LogWarning("SubmitDirection failed: User={UserId} (StaffId={StaffId}) is not the supervisor for Direction={DirectionId}",
                userId.Value, staff?.Id, request.Id);
            return Result.Failure(new Error(
                "403",
                "Only the supervisor who created this direction can submit it."));
        }
        // Get Draft state to verify current state
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
        {
            _logger.LogError("SubmitDirection failed: Draft state not found for WorkType ID={WorkTypeId}", direction.WorkTypeId);
            return Result.Failure(new Error(
                "404",
                "Draft state not found for this work type."));
        }

        // Verify direction is in draft state
        if (direction.CurrentStateId != draftState.Id)
        {
            _logger.LogWarning("SubmitDirection failed: Direction ID={DirectionId} is in state {StateId}, not Draft.",
                request.Id, direction.CurrentStateId);
            return Result.Failure(new Error(
                "409",
                "Only draft directions can be submitted. Current state does not allow submission."));
        }

        // Get Submitted state
        var submittedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Submitted", cancellationToken);

        if (submittedState is null)
        {
            _logger.LogError("SubmitDirection failed: Submitted state not found for WorkType ID={WorkTypeId}", direction.WorkTypeId);
            return Result.Failure(new Error(
                "404",
                "Submitted state not found for this work type."));
        }

        try
        {
            // Submit using domain method (raises DirectionSubmittedEvent)
            direction.Submit(submittedState.Id);

            // Save changes
            await _directionRepository.UpdateAsync(direction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully submitted direction ID={DirectionId}", request.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SubmitDirection failed for ID={DirectionId}", request.Id);
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}