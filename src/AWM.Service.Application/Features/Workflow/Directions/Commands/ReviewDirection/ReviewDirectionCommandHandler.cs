using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.ReviewDirection;

/// <summary>
/// Command handler for reviewing (approving/rejecting/revising) a submitted thesis direction.
/// </summary>
public sealed class ReviewDirectionCommandHandler : IRequestHandler<ReviewDirectionCommand, Result<Unit>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewDirectionCommandHandler"/> class.
    /// </summary>
    public ReviewDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the request to review a direction.
    /// </summary>
    public async Task<Result<Unit>> Handle(ReviewDirectionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var reviewerUserId = _currentUserProvider.UserId.Value;

        var direction = await _directionRepository.GetByIdAsync(request.DirectionId, cancellationToken);

        if (direction == null)
            return Result.Failure<Unit>(new Error("Direction.NotFound", "Direction not found."));

        var submittedState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Submitted, cancellationToken);
        if (direction.CurrentStateId != submittedState?.Id)
            return Result.Failure<Unit>(new Error("Direction.InvalidState", "Only submitted directions can be reviewed."));

        switch (request.Decision)
        {
            case ReviewDecision.Approve:
                var approvedState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Approved, cancellationToken);
                if (approvedState == null) return Result.Failure<Unit>(new Error("State.NotFound", "Approved state not found."));
                direction.Approve(approvedState.Id, reviewerUserId);
                break;

            case ReviewDecision.Reject:
                var rejectedState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Rejected, cancellationToken);
                if (rejectedState == null) return Result.Failure<Unit>(new Error("State.NotFound", "Rejected state not found."));
                direction.Reject(rejectedState.Id, reviewerUserId, request.Comment ?? string.Empty);
                break;

            case ReviewDecision.RequireRevision:
                var revisionState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.RequiresRevision, cancellationToken);
                if (revisionState == null) return Result.Failure<Unit>(new Error("State.NotFound", "Requires Revision state not found."));
                direction.RequestRevision(revisionState.Id, reviewerUserId, request.Comment!);
                break;

            default:
                return Result.Failure<Unit>(new Error("Review.InvalidDecision", "Invalid review decision."));
        }

        await _directionRepository.UpdateAsync(direction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to supervisor (CreatedBy)
        string title = "Результат рассмотрения направления";
        string message = "";
        switch (request.Decision)
        {
            case ReviewDecision.Approve:
                message = $"Ваше направление '{direction.TitleRu}' было утверждено.";
                break;
            case ReviewDecision.Reject:
                message = $"Ваше направление '{direction.TitleRu}' было отклонено.";
                if (!string.IsNullOrWhiteSpace(request.Comment))
                {
                    message += $" Комментарий: {request.Comment}";
                }
                break;
            case ReviewDecision.RequireRevision:
                message = $"Ваше направление '{direction.TitleRu}' требует доработки. Комментарий: {request.Comment}";
                break;
        }

        await _notificationService.SendAsync(
            direction.CreatedBy,
            title,
            reviewerUserId,
            message,
            null,
            "Direction",
            direction.Id,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
