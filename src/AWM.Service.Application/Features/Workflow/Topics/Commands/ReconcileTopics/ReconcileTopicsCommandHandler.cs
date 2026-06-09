using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.ReconcileTopics;

/// <summary>
/// Handles <see cref="ReconcileTopicsCommand"/>.
/// Loads all requested topics with applications, validates user has orgUnit access,
/// validates they are in a reconcilable state, and calls Reconcile() on each.
/// Domain events (TopicReconciledEvent) are raised and handled
/// by <see cref="WorkflowNotificationHandlers"/> for notifications.
/// </summary>
public sealed class ReconcileTopicsCommandHandler : IRequestHandler<ReconcileTopicsCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReconcileTopicsCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IEmployeeReadOnlyRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReconcileTopicsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;
        var topics = await _topicRepository.GetByIdsWithApplicationsAsync(request.TopicIds, cancellationToken);

        if (topics.Count != request.TopicIds.Count)
        {
            var foundIds = topics.Select(t => t.Id).ToHashSet();
            var missingIds = request.TopicIds.Where(id => !foundIds.Contains(id)).ToList();
            return Result.Failure(new Error("Topics.NotFound", $"Topics not found: {string.Join(", ", missingIds)}"));
        }

        var orgUnitId = topics.First().OrgUnitId;
        if (topics.Any(t => t.OrgUnitId != orgUnitId))
        {
            return Result.Failure(new Error("Topics.MultipleOrgUnits", "All topics must belong to the same department."));
        }

        var employee = await _employeeRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        var hasOrgUnitAccess = employee?.Positions.Any(p => p.OrgUnitId == orgUnitId) ?? false;
        if (!hasOrgUnitAccess)
        {
            return Result.Failure(new Error(
                "Auth.OrgUnitAccessDenied",
                "You do not have access to this department."));
        }

        foreach (var topic in topics)
        {
            topic.Reconcile(currentUserId);
            await _topicRepository.UpdateAsync(topic, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
