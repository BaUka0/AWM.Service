namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// User notification entity for tracking notifications and read status.
/// </summary>
public class Notification : Entity<long>, IAuditable
{
    public int UserId { get; private set; }
    public int? TemplateId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Body { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public long? RelatedEntityId { get; private set; }
    public bool IsRead { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private Notification() { }

    public Notification(
        int userId,
        string title,
        int createdBy,
        string? body = null,
        int? templateId = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Notification title is required.", nameof(title));

        UserId = userId;
        TemplateId = templateId;
        Title = title;
        Body = body;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
        IsRead = false;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Marks the notification as read.
    /// </summary>
    public void MarkAsRead()
    {
        IsRead = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the notification as unread.
    /// </summary>
    public void MarkAsUnread()
    {
        IsRead = false;
        LastModifiedAt = DateTime.UtcNow;
    }
}
