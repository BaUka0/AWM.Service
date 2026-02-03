namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.CommonDomain.Entities;

/// <summary>
/// Repository for user notifications (колокольчик).
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Gets a notification by ID.
    /// </summary>
    Task<Notification?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unread notifications for a user.
    /// </summary>
    Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent notifications for a user (paginated).
    /// </summary>
    Task<IReadOnlyList<Notification>> GetByUserAsync(
        int userId, 
        int skip = 0, 
        int take = 20, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of unread notifications.
    /// </summary>
    Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all notifications as read for a user.
    /// </summary>
    Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);

    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}
