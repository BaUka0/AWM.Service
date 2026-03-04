namespace AWM.Service.Application.Features.Thesis.Topics.Commands.SubmitTopicsForApproval;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for batch submission of topics for department approval.
/// </summary>
public sealed class SubmitTopicsForApprovalCommandHandler
    : IRequestHandler<SubmitTopicsForApprovalCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitTopicsForApprovalCommandHandler> _logger;

    public SubmitTopicsForApprovalCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<SubmitTopicsForApprovalCommandHandler> logger)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        SubmitTopicsForApprovalCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to submit {Count} topics for approval by User={UserId}",
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

            try
            {
                topic.SubmitForApproval();
                await _topicRepository.UpdateAsync(topic, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                failedTopics.Add($"Topic {topicId}: {ex.Message}");
            }
        }

        if (failedTopics.Count == request.TopicIds.Count)
        {
            _logger.LogWarning("SubmitTopicsForApproval failed: All topics failed - {Errors}", string.Join("; ", failedTopics));
            return Result.Failure(new Error("400", $"All topics failed: {string.Join("; ", failedTopics)}"));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (failedTopics.Count > 0)
        {
            _logger.LogWarning("SubmitTopicsForApproval partially succeeded. Failed: {Errors}", string.Join("; ", failedTopics));
        }

        _logger.LogInformation("Successfully submitted topics for approval by User={UserId}", userId);
        return Result.Success();
    }
}
