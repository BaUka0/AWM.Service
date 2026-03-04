namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicCoordinationSummary;

/// <summary>
/// DTO for topic coordination summary.
/// </summary>
public sealed record TopicCoordinationSummaryDto
{
    /// <summary>Total number of topics in the department for the year.</summary>
    public int TotalTopics { get; init; }

    /// <summary>Number of approved topics.</summary>
    public int ApprovedTopics { get; init; }

    /// <summary>Number of topics with at least one accepted student.</summary>
    public int TopicsWithStudents { get; init; }

    /// <summary>Number of topics without any accepted student.</summary>
    public int TopicsWithoutStudents { get; init; }

    /// <summary>Number of closed topics.</summary>
    public int ClosedTopics { get; init; }

    /// <summary>Total number of accepted applications.</summary>
    public int TotalAcceptedApplications { get; init; }

    /// <summary>Total available spots across all open topics.</summary>
    public int TotalAvailableSpots { get; init; }

    /// <summary>Per-topic details for coordination review.</summary>
    public IReadOnlyList<TopicCoordinationItemDto> Topics { get; init; } = [];
}

/// <summary>
/// Per-topic detail within the coordination summary.
/// </summary>
public sealed record TopicCoordinationItemDto
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
