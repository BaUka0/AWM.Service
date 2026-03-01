namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CloseTopic;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for closing a thesis topic.
/// Closed topics no longer accept new student applications.
/// </summary>
public sealed class CloseTopicCommandHandler : IRequestHandler<CloseTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<CloseTopicCommandHandler> _logger;

    public CloseTopicCommandHandler(
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ILogger<CloseTopicCommandHandler> logger)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(CloseTopicCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to close topic ID={TopicId} by User={UserId}", request.TopicId, userId);

        try
        {
            // 1. Find the topic
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                _logger.LogWarning("CloseTopic failed: Topic ID={TopicId} not found.", request.TopicId);
                return Result.Failure(new Error("404", $"Topic with ID {request.TopicId} not found."));
            }

            // 2. Business rule: Cannot close already closed topics
            if (topic.IsClosed)
            {
                _logger.LogWarning("CloseTopic failed: Topic ID={TopicId} is already closed.", request.TopicId);
                return Result.Failure(new Error(
                    "409",
                    "This topic is already closed."));
            }

            // 3. Business rule (optional): Topic must be approved before closing
            // This prevents closing draft topics that haven't been reviewed
            if (!topic.IsApproved)
            {
                _logger.LogWarning("CloseTopic failed: Topic ID={TopicId} is not approved.", request.TopicId);
                return Result.Failure(new Error(
                    "409",
                    "Cannot close an unapproved topic. Only approved topics can be closed."));
            }

            // 4. Business rule (optional): Check if all spots are filled
            // This is just a warning - supervisor can still close even if spots are available
            var acceptedCount = topic.Applications.Count(a => a.Status == Domain.Thesis.Enums.ApplicationStatus.Accepted);
            var isFull = acceptedCount >= topic.MaxParticipants;

            // Note: We could add a warning log here or return additional info
            // For now, we allow closing regardless of fill status

            // 5. Close topic using domain method
            topic.Close();

            // Note: The domain entity raises TopicClosedEvent which can be handled
            // by event handlers for notifications to students who applied, etc.

            // 6. Save changes
            await _topicRepository.UpdateAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully closed topic ID={TopicId}", request.TopicId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CloseTopic failed for ID={TopicId}", request.TopicId);
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}