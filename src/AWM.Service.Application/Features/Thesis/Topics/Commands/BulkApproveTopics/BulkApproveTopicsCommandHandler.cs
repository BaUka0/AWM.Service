namespace AWM.Service.Application.Features.Thesis.Topics.Commands.BulkApproveTopics;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for bulk approval of topics by the department.
/// </summary>
public sealed class BulkApproveTopicsCommandHandler
    : IRequestHandler<BulkApproveTopicsCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkApproveTopicsCommandHandler> _logger;

    public BulkApproveTopicsCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<BulkApproveTopicsCommandHandler> logger)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        BulkApproveTopicsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to bulk approve {Count} topics by User={UserId}",
            request.TopicIds.Count, userId);

        if (!userId.HasValue)
        {
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        if (request.TopicIds.Count == 0)
        {
            return Result.Failure(new Error("400", "At least one topic ID must be provided."));
        }

        var failedTopics = new List<string>();

        foreach (var topicId in request.TopicIds)
        {
            var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
            if (topic is null)
            {
                failedTopics.Add($"Topic {topicId} not found.");
                continue;
            }

            if (topic.IsDeleted)
            {
                failedTopics.Add($"Topic {topicId} is deleted.");
                continue;
            }

            if (topic.IsApproved)
            {
                failedTopics.Add($"Topic {topicId} is already approved.");
                continue;
            }

            if (topic.IsClosed)
            {
                failedTopics.Add($"Topic {topicId} is closed.");
                continue;
            }

            topic.Approve();
            await _topicRepository.UpdateAsync(topic, cancellationToken);
        }

        if (failedTopics.Count == request.TopicIds.Count)
        {
            _logger.LogWarning("BulkApproveTopics failed: All topics failed - {Errors}", string.Join("; ", failedTopics));
            return Result.Failure(new Error("400", $"All topics failed: {string.Join("; ", failedTopics)}"));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (failedTopics.Count > 0)
        {
            _logger.LogWarning("BulkApproveTopics partially succeeded. Failed: {Errors}", string.Join("; ", failedTopics));
        }

        _logger.LogInformation("Successfully bulk approved topics by User={UserId}", userId);
        return Result.Success();
    }
}
