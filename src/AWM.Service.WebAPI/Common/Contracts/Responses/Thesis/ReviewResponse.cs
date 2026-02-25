namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for an external review.
/// </summary>
public sealed record ReviewResponse
{
    /// <summary>
    /// Review ID.
    /// </summary>
    /// <example>2</example>
    public long Id { get; init; }

    /// <summary>
    /// ID of the associated StudentWork.
    /// </summary>
    /// <example>10</example>
    public long WorkId { get; init; }

    /// <summary>
    /// User ID of the assigned external reviewer.
    /// </summary>
    /// <example>55</example>
    public int ReviewerId { get; init; }

    /// <summary>
    /// Text content of the review (nullable).
    /// </summary>
    /// <example>Работа заслуживает высокой оценки...</example>
    public string? ReviewText { get; init; }

    /// <summary>
    /// Storage path/key of the attached file, if any.
    /// </summary>
    /// <example>2025/03/another-guid.pdf</example>
    public string? FileStoragePath { get; init; }

    /// <summary>
    /// Indicates whether the review has been uploaded yet.
    /// </summary>
    /// <example>true</example>
    public bool IsUploaded { get; init; }

    /// <summary>
    /// Date when the review assignment was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who created the assignment.
    /// </summary>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date when the review was uploaded/modified (nullable).
    /// </summary>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID who uploaded/modified the review (nullable).
    /// </summary>
    public int? LastModifiedBy { get; init; }
}
