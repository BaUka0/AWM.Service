namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for a file attachment.
/// </summary>
public sealed record AttachmentResponse
{
    /// <summary>
    /// Attachment ID.
    /// </summary>
    /// <example>10</example>
    public long Id { get; init; }

    /// <summary>
    /// ID of the StudentWork this attachment belongs to.
    /// </summary>
    /// <example>5</example>
    public long WorkId { get; init; }

    /// <summary>
    /// Work state ID at time of upload (nullable).
    /// </summary>
    /// <example>2</example>
    public int? StateId { get; init; }

    /// <summary>
    /// Attachment type (Draft, Final, Presentation, Software).
    /// </summary>
    /// <example>Final</example>
    public string AttachmentType { get; init; } = null!;

    /// <summary>
    /// Original file name as uploaded.
    /// </summary>
    /// <example>thesis_final_v3.pdf</example>
    public string FileName { get; init; } = null!;

    /// <summary>
    /// Storage path / key used to retrieve or delete the file.
    /// </summary>
    /// <example>2025/02/a3f1bc44-091e-4f3e-8d0a-1234567890ab.pdf</example>
    public string FileStoragePath { get; init; } = null!;

    /// <summary>
    /// Date and time when the file was uploaded (UTC).
    /// </summary>
    /// <example>2025-02-20T08:45:00Z</example>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who uploaded the file.
    /// </summary>
    /// <example>42</example>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date and time of the last modification (nullable).
    /// </summary>
    /// <example>2025-02-21T10:00:00Z</example>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID who last modified the attachment record (nullable).
    /// </summary>
    /// <example>42</example>
    public int? LastModifiedBy { get; init; }
}
