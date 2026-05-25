using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.ReviewTopic;

public sealed class ReviewTopicCommandHandler : IRequestHandler<ReviewTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewTopicCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReviewTopicCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;
        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

        if (topic == null)
            return Result.Failure(new Error("Topics.NotFound", "Topic not found."));

        // Validation: Topic must be submitted for approval
        if (topic.Status != Domain.Thesis.Enums.TopicStatus.Pending && topic.Status != Domain.Thesis.Enums.TopicStatus.Approved)
            return Result.Failure(new Error("Topics.NotSubmitted", "Topic is not submitted for approval yet."));

        if (request.IsApproved)
        {
            topic.Approve(currentUserId);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.Comment))
                return Result.Failure(new Error("Topics.CommentRequired", "Comment is required when rejecting a topic."));

            topic.Reject(currentUserId, request.Comment);
        }

        await _topicRepository.UpdateAsync(topic, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
