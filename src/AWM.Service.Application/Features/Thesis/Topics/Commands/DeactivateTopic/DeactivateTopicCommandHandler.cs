namespace AWM.Service.Application.Features.Thesis.Topics.Commands.DeactivateTopic;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for deactivating a topic without accepted students.
/// Closes the topic and marks it as deleted.
/// </summary>
public sealed class DeactivateTopicCommandHandler
    : IRequestHandler<DeactivateTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateTopicCommandHandler> _logger;

    public DeactivateTopicCommandHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateTopicCommandHandler> logger)
    {
        _topicRepository = topicRepository;
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        DeactivateTopicCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to deactivate topic ID={TopicId} by User={UserId}",
            request.TopicId, userId);

        if (!userId.HasValue)
        {
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic is null)
        {
            return Result.Failure(new Error("404", $"Topic with ID {request.TopicId} not found."));
        }

        if (topic.IsDeleted)
        {
            return Result.Failure(new Error("409", "Topic is already deactivated."));
        }

        // Check that topic has no accepted students
        var applications = await _applicationRepository.GetByTopicIdAsync(topic.Id, cancellationToken);
        var hasAccepted = applications.Any(a => a.Status == ApplicationStatus.Accepted);

        if (hasAccepted)
        {
            return Result.Failure(new Error("409",
                "Cannot deactivate a topic with accepted students. Remove student assignments first."));
        }

        // Close and soft-delete the topic
        if (!topic.IsClosed)
        {
            topic.Close();
        }

        topic.Delete(userId.Value);

        await _topicRepository.UpdateAsync(topic, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deactivated topic ID={TopicId}", request.TopicId);
        return Result.Success();
    }
}
