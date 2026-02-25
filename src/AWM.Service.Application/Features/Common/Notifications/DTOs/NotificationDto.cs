namespace AWM.Service.Application.Features.Common.Notifications.DTOs;

public sealed record NotificationDto
{
    public long Id { get; init; }
    public int UserId { get; init; }
    public int? TemplateId { get; init; }
    public string Title { get; init; } = null!;
    public string? Body { get; init; }
    public string? RelatedEntityType { get; init; }
    public long? RelatedEntityId { get; init; }
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}
