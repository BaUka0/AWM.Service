namespace AWM.Service.Domain.CommonDomain.Services;

/// <summary>
/// Application-layer service for creating and managing user notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    Task SendAsync(
        int userId,
        string title,
        int createdBy,
        string? body = null,
        int? templateId = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to multiple users.
    /// </summary>
    Task SendToManyAsync(
        IEnumerable<int> userIds,
        string title,
        int createdBy,
        string? body = null,
        int? templateId = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default);
}
