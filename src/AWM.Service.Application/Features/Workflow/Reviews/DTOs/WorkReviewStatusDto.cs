namespace AWM.Service.Application.Features.Workflow.Reviews.DTOs;

public record WorkReviewStatusDto(
    long WorkId,
    string StudentName,
    string TopicTitle,
    string SupervisorName,
    string ReviewerName,
    bool IsSupervisorReviewSubmitted,
    bool IsReviewerReviewSubmitted);
