using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Infrastructure.Services;

/// <summary>
/// Service implementation for managing and sending notifications.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationService"/> class.
    /// </summary>
    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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
        var notifications = userIds.Select(userId => new Notification(
            userId,
            title,
            createdBy,
            body,
            templateId,
            relatedEntityType,
            relatedEntityId)).ToList();

        await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
    }
}
