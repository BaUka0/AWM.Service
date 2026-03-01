namespace AWM.Service.Application.Features.Thesis.Topics.EventHandlers;

using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Events;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Sends notification to the supervisor when a topic is approved by the department.
/// </summary>
public sealed class TopicApprovedNotificationHandler : INotificationHandler<TopicApprovedEvent>
{
    private readonly ITopicRepository _topicRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<TopicApprovedNotificationHandler> _logger;

    public TopicApprovedNotificationHandler(
        ITopicRepository topicRepository,
        INotificationService notificationService,
        ILogger<TopicApprovedNotificationHandler> logger)
    {
        _topicRepository = topicRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(TopicApprovedEvent notification, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(notification.TopicId, cancellationToken);
        if (topic is null) return;

        await _notificationService.SendAsync(
            userId: topic.SupervisorId,
            title: "Тема утверждена",
            createdBy: topic.SupervisorId,
            body: $"Ваша тема «{topic.TitleRu}» была утверждена кафедрой и доступна для выбора студентами.",
            relatedEntityType: "Topic",
            relatedEntityId: topic.Id,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Notification sent to supervisor {SupervisorId} about topic {TopicId} approval",
            topic.SupervisorId, topic.Id);
    }
}
