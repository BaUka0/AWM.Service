using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.UpdateDirection;

/// <summary>
/// Command handler for updating an existing thesis direction.
/// </summary>
public sealed class UpdateDirectionCommandHandler : IRequestHandler<UpdateDirectionCommand, Result<Unit>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDirectionCommandHandler"/> class.
    /// </summary>
    public UpdateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the request to update a direction.
    /// </summary>
    public async Task<Result<Unit>> Handle(UpdateDirectionCommand request, CancellationToken cancellationToken)
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
            return Result.Failure<Unit>(new Error("Direction.Unauthorized", "You can only update your own directions."));

        var draftState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Draft, cancellationToken);
        var revisionState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.RequiresRevision, cancellationToken);

        if (direction.CurrentStateId != draftState?.Id && direction.CurrentStateId != revisionState?.Id)
            return Result.Failure<Unit>(new Error("Direction.InvalidState", "Direction can only be updated in Draft or Requires Revision state."));

        direction.UpdateContent(
            request.TitleRu,
            request.TitleKz,
            request.TitleEn,
            request.DescriptionRu,
            request.DescriptionKz,
            request.DescriptionEn);

        await _directionRepository.UpdateAsync(direction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
