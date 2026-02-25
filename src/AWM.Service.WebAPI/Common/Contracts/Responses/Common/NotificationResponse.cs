namespace AWM.Service.WebAPI.Common.Contracts.Responses.Common;

/// <summary>
/// Response contract for a single user notification.
/// </summary>
public sealed record NotificationResponse
{
    /// <summary>
    /// Notification ID.
    /// </summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>
    /// ID of the user this notification belongs to.
    /// </summary>
    /// <example>42</example>
    public int UserId { get; init; }

    /// <summary>
    /// Optional template ID used to generate this notification.
    /// </summary>
    /// <example>5</example>
    public int? TemplateId { get; init; }

    /// <summary>
    /// Short notification title.
    /// </summary>
    /// <example>Your thesis topic has been approved</example>
    public string Title { get; init; } = null!;

    /// <summary>
    /// Optional notification body / detailed message.
    /// </summary>
    /// <example>Your thesis topic "Development of AWM" was approved by your supervisor.</example>
    public string? Body { get; init; }

    /// <summary>
    /// Type of the related entity (e.g. "Topic", "StudentWork").
    /// </summary>
    /// <example>Topic</example>
    public string? RelatedEntityType { get; init; }

    /// <summary>
    /// ID of the related entity.
    /// </summary>
    /// <example>100</example>
    public long? RelatedEntityId { get; init; }

    /// <summary>
    /// Whether the notification has been read by the user.
    /// </summary>
    /// <example>false</example>
    public bool IsRead { get; init; }

    /// <summary>
    /// Date and time when the notification was created (UTC).
    /// </summary>
    /// <example>2024-03-01T09:15:00Z</example>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who created (sent) the notification.
    /// </summary>
    /// <example>1</example>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date and time when the notification was last modified (nullable).
    /// </summary>
    /// <example>2024-03-02T11:00:00Z</example>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID who last modified the notification (nullable).
    /// </summary>
    /// <example>1</example>
    public int? LastModifiedBy { get; init; }
}
