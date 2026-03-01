namespace AWM.Service.Application.Features.Thesis.Topics.Commands.UpdateTopic;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for updating an existing thesis topic.
/// </summary>
public sealed class UpdateTopicCommandHandler : IRequestHandler<UpdateTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<UpdateTopicCommandHandler> _logger;

    public UpdateTopicCommandHandler(
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ILogger<UpdateTopicCommandHandler> logger)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("Attempting to update topic ID={TopicId} by User={UserId}", request.TopicId, userId);

            if (!userId.HasValue)
            {
                _logger.LogWarning("UpdateTopic failed: User ID is not available.");
                return Result.Failure(new Error("401", "User ID is not available."));
            }

            // 2. Find the topic
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                _logger.LogWarning("UpdateTopic failed: Topic ID={TopicId} not found.", request.TopicId);
                return Result.Failure(new Error("404", $"Topic with ID {request.TopicId} not found."));
            }

            // 3. Business rule: Cannot update approved topics
            if (topic.IsApproved)
            {
                _logger.LogWarning("UpdateTopic failed: Topic ID={TopicId} is already approved.", request.TopicId);
                return Result.Failure(new Error(
                    "409",
                    "Cannot update an approved topic. Please revoke approval first."));
            }

            // 4. Business rule: Cannot update closed topics
            if (topic.IsClosed)
            {
                _logger.LogWarning("UpdateTopic failed: Topic ID={TopicId} is closed.", request.TopicId);
                return Result.Failure(new Error(
                    "409",
                    "Cannot update a closed topic. Please reopen the topic first."));
            }

            // 5. Business rule: If reducing MaxParticipants, check if it's possible
            var acceptedApplicationsCount = topic.Applications.Count(a => a.Status == Domain.Thesis.Enums.ApplicationStatus.Accepted);

            if (request.MaxParticipants < acceptedApplicationsCount)
            {
                _logger.LogWarning("UpdateTopic failed: Requested MaxParticipants={Requested} < Accepted={Accepted} for Topic ID={TopicId}",
                    request.MaxParticipants, acceptedApplicationsCount, request.TopicId);
                return Result.Failure(new Error(
                    "409",
                    $"Cannot reduce max participants to {request.MaxParticipants}. There are already {acceptedApplicationsCount} accepted applications."));
            }

            // 6. Update topic content using domain methods
            topic.UpdateContent(
                request.TitleRu,
                request.TitleKz,
                request.TitleEn,
                request.Description);

            // 7. Update max participants
            topic.UpdateMaxParticipants(request.MaxParticipants);

            // 8. Update metadata
            // Note: Domain entity should handle LastModifiedAt/LastModifiedBy automatically
            // If not, we'd need to add a method like topic.UpdateMetadata(modifiedBy)

            // 9. Save changes
            await _topicRepository.UpdateAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated topic ID={TopicId}", request.TopicId);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "UpdateTopic validation failed for ID={TopicId}: {Message}", request.TopicId, argEx.Message);
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateTopic failed for ID={TopicId}", request.TopicId);
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}