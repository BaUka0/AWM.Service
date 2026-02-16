namespace AWM.Service.Application.Features.Thesis.Directions.Commands.SubmitDirection;

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

    public SubmitDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
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
                "Direction.NotFound", 
                $"Direction with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (direction.IsDeleted)
        {
            return Result.Failure(new Error(
                "Direction.Deleted", 
                $"Direction with ID {request.Id} has been deleted."));
        }

        // Verify user is the supervisor (authorization check)
        if (direction.SupervisorId != request.SubmittedBy)
        {
            return Result.Failure(new Error(
                "Direction.Unauthorized", 
                "Only the supervisor who created this direction can submit it."));
        }

        // Get Draft state to verify current state
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
        {
            return Result.Failure(new Error(
                "State.NotFound", 
                "Draft state not found for this work type."));
        }

        // Verify direction is in draft state
        if (direction.CurrentStateId != draftState.Id)
        {
            return Result.Failure(new Error(
                "Direction.InvalidState", 
                "Only draft directions can be submitted. Current state does not allow submission."));
        }

        // Get Submitted state
        var submittedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Submitted", cancellationToken);

        if (submittedState is null)
        {
            return Result.Failure(new Error(
                "State.NotFound", 
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
            return Result.Failure(new Error("InternalError", ex.Message));
        }
    }
}