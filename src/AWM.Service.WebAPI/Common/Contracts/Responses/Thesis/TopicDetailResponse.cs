namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for topic data with full details including applications.
/// </summary>
public sealed record TopicDetailResponse
{
    /// <summary>
    /// Topic ID.
    /// </summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>
    /// Direction ID (nullable).
    /// </summary>
    /// <example>10</example>
    public long? DirectionId { get; init; }

    /// <summary>
    /// Department ID.
    /// </summary>
    /// <example>1</example>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Supervisor ID.
    /// </summary>
    /// <example>5</example>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    /// <example>2</example>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Work type ID.
    /// </summary>
    /// <example>1</example>
    public int WorkTypeId { get; init; }

    /// <summary>
    /// Topic title in Russian.
    /// </summary>
    /// <example>Разработка системы управления дипломными проектами</example>
    public string TitleRu { get; init; } = null!;

    /// <summary>
    /// Topic title in English (optional).
    /// </summary>
    /// <example>Development of a Thesis Management System</example>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Topic title in Kazakh (optional).
    /// </summary>
    /// <example>Дипломдық жобаларды басқару жүйесін әзірлеу</example>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Topic description.
    /// </summary>
    /// <example>Система для автоматизации процесса управления дипломными работами студентов</example>
    public string? Description { get; init; }

    /// <summary>
    /// Maximum number of participants.
    /// </summary>
    /// <example>1</example>
    public int MaxParticipants { get; init; }

    /// <summary>
    /// Number of available spots.
    /// </summary>
    /// <example>1</example>
    public int AvailableSpots { get; init; }

    /// <summary>
    /// Whether the topic is approved.
    /// </summary>
    /// <example>true</example>
    public bool IsApproved { get; init; }

    /// <summary>
    /// Whether the topic is closed.
    /// </summary>
    /// <example>false</example>
    public bool IsClosed { get; init; }

    /// <summary>
    /// Whether this is a team topic.
    /// </summary>
    /// <example>false</example>
    public bool IsTeamTopic { get; init; }

    /// <summary>
    /// Date and time when the topic was created.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who created the topic.
    /// </summary>
    /// <example>5</example>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date and time when the topic was last modified (nullable).
    /// </summary>
    /// <example>2024-02-01T14:20:00Z</example>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID who last modified the topic (nullable).
    /// </summary>
    /// <example>5</example>
    public int? LastModifiedBy { get; init; }

    /// <summary>
    /// List of student applications for this topic.
    /// </summary>
    public IReadOnlyCollection<TopicApplicationResponse>? Applications { get; init; }
}
