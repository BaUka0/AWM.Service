namespace AWM.Service.Application.Features.Workflow.Reviews.DTOs;

public record ReviewerAssignmentDto(
    long WorkId,
    bool IsReviewUploaded,
    string TopicTitle,
    string StudentName,
    long? ReviewId);
