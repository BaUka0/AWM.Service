namespace AWM.Service.WebAPI.Common.Contracts.Responses.Topics;

/// <summary>
/// Response with reconciliation summary and topic list for the department.
/// </summary>
public record TopicReconciliationSummaryResponse(
    int TotalTopics,
    int TopicsWithAcceptedStudents,
    int TopicsWithoutStudents,
    int TopicsWithExcessApplications,
    int ReconciledTopics,
    int InactiveTopics,
    int NeedsRevisionTopics,
    IReadOnlyList<TopicReconciliationItemResponse> Topics);
