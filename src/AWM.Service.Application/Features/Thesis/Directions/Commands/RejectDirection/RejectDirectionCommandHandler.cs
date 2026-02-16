namespace AWM.Service.Application.Features.Thesis.Directions.Commands.RejectDirection;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for rejecting a direction.
/// </summary>
public sealed class RejectDirectionCommandHandler 
    : IRequestHandler<RejectDirectionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public RejectDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(
        RejectDirectionCommand request, 
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

        // Get Submitted state to verify current state
        var submittedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Submitted", cancellationToken);

        if (submittedState is null)
        {
            return Result.Failure(new Error(
                "State.NotFound", 
                "Submitted state not found for this work type."));
        }

        // Verify direction is in submitted state
        if (direction.CurrentStateId != submittedState.Id)
        {
            return Result.Failure(new Error(
                "Direction.InvalidState", 
                "Only submitted directions can be rejected. Current state does not allow rejection."));
        }

        // Get Rejected state
        var rejectedState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Rejected", cancellationToken);

        if (rejectedState is null)
        {
            return Result.Failure(new Error(
                "State.NotFound", 
                "Rejected state not found for this work type."));
        }

        try
        {
            // Reject using domain method (raises DirectionRejectedEvent)
            direction.Reject(rejectedState.Id, request.RejectedBy, request.Comment);

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