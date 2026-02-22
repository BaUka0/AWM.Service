namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for topic application data.
/// </summary>
public sealed record TopicApplicationResponse
{
    /// <summary>
    /// Application ID.
    /// </summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>
    /// Topic ID.
    /// </summary>
    /// <example>10</example>
    public long TopicId { get; init; }

    /// <summary>
    /// Student ID.
    /// </summary>
    /// <example>100</example>
    public int StudentId { get; init; }

    /// <summary>
    /// Optional motivation letter.
    /// </summary>
    public string? MotivationLetter { get; init; }

    /// <summary>
    /// Date when the application was submitted.
    /// </summary>
    /// <example>2024-09-15T10:30:00Z</example>
    public DateTime AppliedAt { get; init; }

    /// <summary>
    /// Application status as string.
    /// </summary>
    /// <example>Submitted</example>
    public string Status { get; init; } = null!;

    /// <summary>
    /// Whether the application is pending review.
    /// </summary>
    /// <example>true</example>
    public bool IsPending { get; init; }

    /// <summary>
    /// Whether the application was accepted.
    /// </summary>
    /// <example>false</example>
    public bool IsAccepted { get; init; }

    /// <summary>
    /// Date when application was reviewed (if reviewed).
    /// </summary>
    public DateTime? ReviewedAt { get; init; }

    /// <summary>
    /// ID of the reviewer (if reviewed).
    /// </summary>
    public int? ReviewedBy { get; init; }

    /// <summary>
    /// Review comment (rejection reason or acceptance note).
    /// </summary>
    public string? ReviewComment { get; init; }
}
