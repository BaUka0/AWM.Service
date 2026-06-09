using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.SubmitDirection;

/// <summary>
/// Command handler for submitting a thesis direction.
/// </summary>
public sealed class SubmitDirectionCommandHandler : IRequestHandler<SubmitDirectionCommand, Result<Unit>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitDirectionCommandHandler"/> class.
    /// </summary>
    public SubmitDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the request to submit a direction.
    /// </summary>
    public async Task<Result<Unit>> Handle(SubmitDirectionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var direction = await _directionRepository.GetByIdAsync(request.DirectionId, cancellationToken);

        if (direction == null)
            return Result.Failure<Unit>(new Error("Direction.NotFound", "Direction not found."));

        if (direction.CreatedBy != currentUserId)
            return Result.Failure<Unit>(new Error("Direction.Unauthorized", "You can only submit your own directions."));

        var (isAllowed, errorMessage) = await _stageValidationService.ValidateOperationInStageAsync(
            direction.OrgUnitId,
            direction.SemesterId,
            WorkflowStageIds.DirectionProposal,
            cancellationToken: cancellationToken);

        if (!isAllowed)
        {
            return Result.Failure<Unit>(new Error("Directions.StageClosed", errorMessage ?? "The direction formation stage is closed."));
        }

        var draftState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Draft, cancellationToken);
        var revisionState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.RequiresRevision, cancellationToken);

        if (direction.CurrentStateId != draftState?.Id && direction.CurrentStateId != revisionState?.Id)
            return Result.Failure<Unit>(new Error("Direction.InvalidState", "Direction must be in Draft or Requires Revision state to be submitted."));

        var submittedState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Submitted, cancellationToken);
        if (submittedState == null)
            return Result.Failure<Unit>(new Error("State.NotFound", "Submitted state not found."));

        direction.Submit(submittedState.Id);

        await _directionRepository.UpdateAsync(direction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
