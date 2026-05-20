namespace AWM.Service.WebAPI.Common.Contracts.Responses.Common;

/// <summary>
/// Response contract for a list of user notifications.
/// </summary>
public sealed class NotificationListResponse
{
    /// <summary>
    /// Total count of unread notifications for the current user.
    /// </summary>
    /// <example>3</example>
    public int UnreadCount { get; init; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Paginated list of notifications.
    /// </summary>
    public IReadOnlyList<NotificationResponse> Items { get; init; } = Array.Empty<NotificationResponse>();
}
