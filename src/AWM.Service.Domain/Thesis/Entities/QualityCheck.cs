namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// QualityCheck entity - results of quality checks (e.g. NormControl, SoftwareCheck, AntiPlagiarism).
/// Supports retry cycle with attempt numbering.
/// </summary>
public class QualityCheck : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int CheckTypeId { get; private set; }
    public int? AssignedExpertId { get; private set; }
    public int AttemptNumber { get; private set; }
    public bool IsPassed { get; private set; }
    public decimal? ResultValue { get; private set; }
    public string? Comment { get; private set; }
    public long? AttachmentId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Navigation properties
    public CheckType? CheckType { get; private set; }
    public Attachment? Attachment { get; private set; }

    // Legacy field
    public DateTime CheckedAt => CreatedAt;

    private QualityCheck() { }

    internal QualityCheck(
        long workId,
        int checkTypeId,
        bool isPassed,
        int attemptNumber = 1,
        int? expertId = null,
        decimal? resultValue = null,
        string? comment = null,
        long? attachmentId = null)
    {
        WorkId = workId;
        CheckTypeId = checkTypeId;
        IsPassed = isPassed;
        AttemptNumber = attemptNumber;
        AssignedExpertId = expertId;
        ResultValue = resultValue;
        Comment = comment;
        AttachmentId = attachmentId;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = expertId ?? 0;
    }

    /// <summary>
    /// Records the expert's result on a pending quality check.
    /// Called by StudentWork.CompleteQualityCheck.
    /// </summary>
    internal void SetResult(
        int expertId,
        bool isPassed,
        decimal? resultValue = null,
        string? comment = null,
        long? attachmentId = null)
    {
        AssignedExpertId = expertId;
        IsPassed = isPassed;
        ResultValue = resultValue;
        Comment = comment;
        AttachmentId = attachmentId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = expertId;
    }

    /// <summary>
    /// Updates the attachment for this quality check.
    /// </summary>
    internal void UpdateAttachmentId(long attachmentId, int modifiedBy)
    {
        AttachmentId = attachmentId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Checks if this check has a numeric result (e.g. percentage).
    /// </summary>
    public bool HasNumericResult => ResultValue.HasValue;

    /// <summary>
    /// Gets the numeric result (e.g. percentage).
    /// </summary>
    public decimal? GetNumericResult()
    {
        return ResultValue;
    }
}
