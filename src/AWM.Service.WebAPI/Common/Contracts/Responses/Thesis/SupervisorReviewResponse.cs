namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for a supervisor review.
/// </summary>
public sealed record SupervisorReviewResponse
{
    /// <summary>
    /// Review ID.
    /// </summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>
    /// ID of the associated StudentWork.
    /// </summary>
    /// <example>10</example>
    public long WorkId { get; init; }

    /// <summary>
    /// User ID of the supervisor.
    /// </summary>
    /// <example>42</example>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Text content of the review.
    /// </summary>
    /// <example>Студент показал отличные результаты...</example>
    public string ReviewText { get; init; } = null!;

    /// <summary>
    /// Storage path/key of the attached file, if any.
    /// </summary>
    /// <example>2025/03/some-guid.pdf</example>
    public string? FileStoragePath { get; init; }

    /// <summary>
    /// Date when the review was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who created the review.
    /// </summary>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date when the review was last modified (nullable).
    /// </summary>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID to last modify the review (nullable).
    /// </summary>
    public int? LastModifiedBy { get; init; }
}
