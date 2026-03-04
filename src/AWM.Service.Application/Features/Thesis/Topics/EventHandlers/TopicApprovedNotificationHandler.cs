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
    private readonly IStaffRepository _staffRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<TopicApprovedNotificationHandler> _logger;

    public TopicApprovedNotificationHandler(
        ITopicRepository topicRepository,
        IStaffRepository staffRepository,
        INotificationService notificationService,
        ILogger<TopicApprovedNotificationHandler> logger)
    {
        _topicRepository = topicRepository;
        _staffRepository = staffRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(TopicApprovedEvent notification, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(notification.TopicId, cancellationToken);
        if (topic is null) return;

        // Resolve Staff -> UserId for notification
        var staff = await _staffRepository.GetByIdAsync(topic.SupervisorId, cancellationToken);
        if (staff is null)
        {
            _logger.LogWarning("Cannot send notification: Staff {StaffId} not found for topic {TopicId}", topic.SupervisorId, topic.Id);
            return;
        }

        await _notificationService.SendAsync(
            userId: staff.UserId,
            title: "Тема утверждена",
            createdBy: staff.UserId,
            body: $"Ваша тема «{topic.TitleRu}» была утверждена кафедрой и доступна для выбора студентами.",
            relatedEntityType: "Topic",
            relatedEntityId: topic.Id,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Notification sent to supervisor UserId={UserId} (StaffId={StaffId}) about topic {TopicId} approval",
            staff.UserId, topic.SupervisorId, topic.Id);
    }
}
