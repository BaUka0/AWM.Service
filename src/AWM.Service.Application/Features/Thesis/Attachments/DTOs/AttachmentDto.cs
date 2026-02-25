namespace AWM.Service.Application.Features.Thesis.Attachments.DTOs;

using AWM.Service.Domain.Thesis.Enums;

public sealed record AttachmentDto
{
    public long Id { get; init; }
    public long WorkId { get; init; }
    public int? StateId { get; init; }
    public AttachmentType AttachmentType { get; init; }
    public string FileName { get; init; } = null!;
    public string FileStoragePath { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}
