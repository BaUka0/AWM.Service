namespace AWM.Service.Application.Features.Thesis.Directions.Commands.RequestRevision;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for requesting revision of a direction.
/// </summary>
public sealed class RequestRevisionCommandHandler 
    : IRequestHandler<RequestRevisionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public RequestRevisionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(
        RequestRevisionCommand request, 
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
                "Only submitted directions can be sent for revision. Current state does not allow this action."));
        }

        // Get Revision state (or RevisionRequested, depending on naming convention)
        var revisionState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Revision", cancellationToken);

        if (revisionState is null)
        {
            return Result.Failure(new Error(
                "State.NotFound", 
                "Revision state not found for this work type."));
        }

        try
        {
            // Request revision using domain method (raises DirectionRequiresRevisionEvent)
            direction.RequestRevision(revisionState.Id, request.RequestedBy, request.Comment);

            // Save changes
            await _directionRepository.UpdateAsync(direction, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (e.g., empty comment)
            return Result.Failure(new Error("Validation.Error", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("InternalError", ex.Message));
        }
    }
}