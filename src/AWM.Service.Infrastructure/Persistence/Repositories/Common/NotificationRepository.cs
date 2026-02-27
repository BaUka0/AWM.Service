namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Notification.
/// </summary>
public sealed class NotificationRepository : RepositoryBase<Notification, long>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notification>> GetByUserAsync(
        int userId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        return await Context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        var unread = await Context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unread)
        {
            notification.MarkAsRead();
        }
    }
}
