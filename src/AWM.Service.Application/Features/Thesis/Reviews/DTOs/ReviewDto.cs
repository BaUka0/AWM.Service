namespace AWM.Service.Application.Features.Thesis.Reviews.DTOs;

public sealed record ReviewDto
{
    public long Id { get; init; }
    public long WorkId { get; init; }
    public int ReviewerId { get; init; }
    public string? ReviewText { get; init; }
    public string? FileStoragePath { get; init; }
    public bool IsUploaded { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}
