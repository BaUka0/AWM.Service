namespace AWM.Service.Application.Features.Thesis.Directions.Commands.SubmitDirection;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for submitting a direction for department review.
/// </summary>
public sealed class SubmitDirectionCommandHandler
    : IRequestHandler<SubmitDirectionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SubmitDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(
        SubmitDirectionCommand request,
        CancellationToken cancellationToken)
    {
        // Get existing direction
        var direction = await _directionRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (direction is null)
        {
            return Result.Failure(new Error(
                "404",
                $"Direction with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (direction.IsDeleted)
        {
            return Result.Failure(new Error(
                "409",
                $"Direction with ID {request.Id} has been deleted."));
        }

        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        // Verify user is the supervisor (authorization check)
        if (direction.SupervisorId != userId.Value)
        {
            return Result.Failure(new Error(
                "403",
                "Only the supervisor who created this direction can submit it."));
        }
        // Get Draft state to verify current state
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
        {
            return Result.Failure(new Error(
                "404",
                "Draft state not found for this work type."));
        }

        // Verify direction is in draft state
        if (direction.CurrentStateId != draftState.Id)
        {
            return Result.Failure(new Error(
                "409",
                "Only draft directions can be submitted. Current state does not allow submission."));
        }

        // Get Submitted state
        var submittedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Submitted", cancellationToken);

        if (submittedState is null)
        {
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

            return Result.Success();
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}