namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response for topic coordination summary.
/// </summary>
public sealed record TopicCoordinationSummaryResponse
{
    public int TotalTopics { get; init; }
    public int ApprovedTopics { get; init; }
    public int TopicsWithStudents { get; init; }
    public int TopicsWithoutStudents { get; init; }
    public int ClosedTopics { get; init; }
    public int TotalAcceptedApplications { get; init; }
    public int TotalAvailableSpots { get; init; }
    public IReadOnlyList<TopicCoordinationItemResponse> Topics { get; init; } = [];
}

/// <summary>
/// Per-topic detail within the coordination summary response.
/// </summary>
public sealed record TopicCoordinationItemResponse
{
    public long TopicId { get; init; }
    public string TitleRu { get; init; } = null!;
    public int SupervisorId { get; init; }
    public int MaxParticipants { get; init; }
    public int AcceptedCount { get; init; }
    public int PendingCount { get; init; }
    public int AvailableSpots { get; init; }
    public bool IsApproved { get; init; }
    public bool IsClosed { get; init; }
}
