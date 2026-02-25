namespace AWM.Service.WebAPI.Common.Contracts.Responses.Common;

/// <summary>
/// Response contract for a list of user notifications.
/// </summary>
public sealed record NotificationListResponse
{
    /// <summary>
    /// Total count of unread notifications for the current user.
    /// </summary>
    /// <example>3</example>
    public int UnreadCount { get; init; }

    /// <summary>
    /// Paginated list of notifications.
    /// </summary>
    public IReadOnlyList<NotificationResponse> Items { get; init; } = [];
}
