using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CloseTopic;

/// <summary>
/// Handles <see cref="CloseTopicCommand"/>.
/// Only the topic creator can close their own approved topics.
/// Closing a topic prevents new applications from being submitted.
/// </summary>
public sealed class CloseTopicCommandHandler : IRequestHandler<CloseTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CloseTopicCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CloseTopicCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic == null)
            return Result.Failure(new Error("Topics.NotFound", $"Topic with ID {request.TopicId} not found."));

        // Only the topic creator can close the topic
        if (topic.CreatedBy != _currentUserProvider.UserId.Value)
            return Result.Failure(new Error("Topics.Forbidden", "Only the topic creator can close the topic."));

        // Domain method handles status validation and raises TopicClosedEvent
        topic.Close();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
