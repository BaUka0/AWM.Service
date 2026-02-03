namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Notification template for different event types.
/// Supports multilingual content (Russian, Kazakh, English).
/// </summary>
public class NotificationTemplate : Entity<int>, IAuditable, ISoftDeletable
{
    public string EventType { get; private set; } = null!;
    public string? TitleRu { get; private set; }
    public string? TitleKz { get; private set; }
    public string? TitleEn { get; private set; }
    public string? BodyTemplateRu { get; private set; }
    public string? BodyTemplateKz { get; private set; }
    public string? BodyTemplateEn { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private NotificationTemplate() { }

    public NotificationTemplate(
        string eventType,
        string titleRu,
        string bodyTemplateRu,
        int createdBy,
        string? titleKz = null,
        string? bodyTemplateKz = null,
        string? titleEn = null,
        string? bodyTemplateEn = null)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type is required.", nameof(eventType));

        EventType = eventType;
        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        BodyTemplateRu = bodyTemplateRu;
        BodyTemplateKz = bodyTemplateKz;
        BodyTemplateEn = bodyTemplateEn;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Soft deletes the template.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Gets the title in the specified language.
    /// </summary>
    public string GetTitle(string languageCode)
    {
        return languageCode.ToLowerInvariant() switch
        {
            "kz" or "kk" => TitleKz ?? TitleRu ?? string.Empty,
            "en" => TitleEn ?? TitleRu ?? string.Empty,
            _ => TitleRu ?? string.Empty
        };
    }

    /// <summary>
    /// Gets the body template in the specified language.
    /// </summary>
    public string GetBodyTemplate(string languageCode)
    {
        return languageCode.ToLowerInvariant() switch
        {
            "kz" or "kk" => BodyTemplateKz ?? BodyTemplateRu ?? string.Empty,
            "en" => BodyTemplateEn ?? BodyTemplateRu ?? string.Empty,
            _ => BodyTemplateRu ?? string.Empty
        };
    }
}
