namespace AWM.Service.Application.Features.Workflow.Commands.TransitionState;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf;
using AWM.Service.Domain.Wf.Services;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class TransitionStateCommandHandler : IRequestHandler<TransitionStateCommand, Result>
{
    private readonly IStateMachine _stateMachine;
    private readonly IDirectionRepository _directionRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public TransitionStateCommandHandler(
        IStateMachine stateMachine,
        IDirectionRepository directionRepository,
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _studentWorkRepository = studentWorkRepository ?? throw new ArgumentNullException(nameof(studentWorkRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(TransitionStateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            return request.EntityType.ToLowerInvariant() switch
            {
                WorkflowEntityTypes.Direction => await TransitionDirectionAsync(request, userId.Value, cancellationToken),
                WorkflowEntityTypes.StudentWork or WorkflowEntityTypes.Work => await TransitionStudentWorkAsync(request, userId.Value, cancellationToken),
                _ => Result.Failure(new Error("400", $"Unknown entity type: {request.EntityType}"))
            };
        }
        catch (InvalidOperationException opEx)
        {
            return Result.Failure(new Error("409", opEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred during state transition: {ex.Message}"));
        }
    }

    private async Task<Result> TransitionDirectionAsync(TransitionStateCommand request, int userId, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(request.EntityId, cancellationToken);
        if (direction is null || direction.IsDeleted)
            return Result.Failure(new Error("404", $"Direction with ID {request.EntityId} not found."));

        var canTransition = await _stateMachine.CanTransitionAsync(direction.CurrentStateId, request.TargetStateId, null, cancellationToken);
        if (!canTransition)
            return Result.Failure(new Error("409", $"Transition from current state to state {request.TargetStateId} is not allowed."));

        // Get the target state to determine what action to take
        var targetState = await _stateMachine.GetStateAsync(request.TargetStateId, cancellationToken);
        if (targetState is null)
            return Result.Failure(new Error("404", $"Target state with ID {request.TargetStateId} not found."));

        // Apply the appropriate domain method based on target state
        switch (targetState.SystemName)
        {
            case Domain.Wf.Entities.DirectionStates.Submitted:
                direction.Submit(request.TargetStateId);
                break;
            case Domain.Wf.Entities.DirectionStates.Approved:
                direction.Approve(request.TargetStateId, userId);
                break;
            case Domain.Wf.Entities.DirectionStates.Rejected:
                direction.Reject(request.TargetStateId, userId, request.Comment);
                break;
            case Domain.Wf.Entities.DirectionStates.RequiresRevision:
                direction.RequestRevision(request.TargetStateId, userId, request.Comment ?? "Revision required");
                break;
            default:
                return Result.Failure(new Error("409", $"Unsupported target state: {targetState.SystemName}"));
        }

        await _directionRepository.UpdateAsync(direction, cancellationToken);
        return Result.Success();
    }

    private async Task<Result> TransitionStudentWorkAsync(TransitionStateCommand request, int userId, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdAsync(request.EntityId, cancellationToken);
        if (work is null || work.IsDeleted)
            return Result.Failure(new Error("404", $"StudentWork with ID {request.EntityId} not found."));

        var canTransition = await _stateMachine.CanTransitionAsync(work.CurrentStateId, request.TargetStateId, null, cancellationToken);
        if (!canTransition)
            return Result.Failure(new Error("409", $"Transition from current state to state {request.TargetStateId} is not allowed."));

        work.ChangeState(request.TargetStateId, userId, request.Comment);
        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        return Result.Success();
    }
}
