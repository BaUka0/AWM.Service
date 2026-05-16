namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for a reviewer assignment item.
/// </summary>
public sealed record ReviewerAssignmentResponse
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
