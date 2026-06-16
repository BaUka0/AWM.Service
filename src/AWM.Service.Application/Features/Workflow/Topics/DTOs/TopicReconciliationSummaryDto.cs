namespace AWM.Service.Application.Features.Workflow.Topics.DTOs;

/// <summary>
/// Summary DTO for the topic reconciliation stage.
/// Provides aggregate statistics and full list of topics for department review.
/// </summary>
public record TopicReconciliationSummaryDto(
    int TotalTopics,
    int TopicsWithAcceptedStudents,
    int TopicsWithoutStudents,
    int TopicsWithExcessApplications,
    int ReconciledTopics,
    int InactiveTopics,
    int NeedsRevisionTopics,
    IReadOnlyList<TopicReconciliationItemDto> Topics);
