namespace AWM.Service.Application.Features.Common.Notifications.Services;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;

/// <summary>
/// Application-layer implementation of INotificationService.
/// Creates and persists notifications for users.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
    }

    public async Task SendAsync(
        int userId,
        string title,
        int createdBy,
        string? body = null,
        int? templateId = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification(
            userId,
            title,
            createdBy,
            body,
            templateId,
            relatedEntityType,
            relatedEntityId);

        await _notificationRepository.AddAsync(notification, cancellationToken);
    }

    public async Task SendToManyAsync(
        IEnumerable<int> userIds,
        string title,
        int createdBy,
        string? body = null,
        int? templateId = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = userIds.Select(userId => SendAsync(
            userId,
            title,
            createdBy,
            body,
            templateId,
            relatedEntityType,
            relatedEntityId,
            cancellationToken));

        await Task.WhenAll(tasks);
    }
}
