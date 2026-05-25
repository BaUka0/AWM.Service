using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.MarkTopicsInactive;

/// <summary>
/// Handles <see cref="MarkTopicsInactiveCommand"/>.
/// Marks topics without student applications as inactive.
/// </summary>
public sealed class MarkTopicsInactiveCommandHandler : IRequestHandler<MarkTopicsInactiveCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public MarkTopicsInactiveCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkTopicsInactiveCommand request, CancellationToken cancellationToken)
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
            // Domain method validates status
            topic.MarkInactive(currentUserId);
            await _topicRepository.UpdateAsync(topic, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
