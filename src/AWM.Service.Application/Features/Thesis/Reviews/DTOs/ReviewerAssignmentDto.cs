namespace AWM.Service.Application.Features.Thesis.Reviews.DTOs;

public sealed record ReviewerAssignmentDto
{
    public long WorkId { get; init; }
    public long ReviewId { get; init; }
    public string? TopicTitle { get; init; }
    public string? StudentName { get; init; }
    public int DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public bool IsReviewUploaded { get; init; }
    public DateTime AssignedAt { get; init; }
    public DateTime? UploadedAt { get; init; }
}
