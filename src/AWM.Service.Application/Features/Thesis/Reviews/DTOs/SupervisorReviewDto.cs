namespace AWM.Service.Application.Features.Thesis.Reviews.DTOs;

public sealed record SupervisorReviewDto
{
    public long Id { get; init; }
    public long WorkId { get; init; }
    public int SupervisorId { get; init; }
    public string ReviewText { get; init; } = null!;
    public string? FileStoragePath { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}
