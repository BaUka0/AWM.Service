using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Events;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.ReconcileTopics;

/// <summary>
/// Handles <see cref="ReconcileTopicsCommand"/>.
/// Loads all requested topics, validates they are in a reconcilable state,
/// and calls Reconcile() on each. Raises a batch TopicsReconciledEvent.
/// </summary>
public sealed class ReconcileTopicsCommandHandler : IRequestHandler<ReconcileTopicsCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ReconcileTopicsCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReconcileTopicsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;
        var topics = await _topicRepository.GetByIdsAsync(request.TopicIds, cancellationToken);

        if (topics.Count != request.TopicIds.Count)
        {
            var foundIds = topics.Select(t => t.Id).ToHashSet();
            var missingIds = request.TopicIds.Where(id => !foundIds.Contains(id)).ToList();
            return Result.Failure(new Error("Topics.NotFound", $"Topics not found: {string.Join(", ", missingIds)}"));
        }

        foreach (var topic in topics)
        {
            // Domain method validates status and raises TopicApprovedEvent
            topic.Reconcile(currentUserId);
            await _topicRepository.UpdateAsync(topic, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
