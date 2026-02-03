namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// QualityCheck entity - results of quality checks (NormControl, SoftwareCheck, AntiPlagiarism).
/// Supports retry cycle with attempt numbering.
/// </summary>
public class QualityCheck : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public CheckType CheckType { get; private set; }
    public int? AssignedExpertId { get; private set; }
    public int AttemptNumber { get; private set; }
    public bool IsPassed { get; private set; }
    public decimal? ResultValue { get; private set; }
    public string? Comment { get; private set; }
    public string? DocumentPath { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Legacy field
    public DateTime CheckedAt => CreatedAt;

    private QualityCheck() { }

    internal QualityCheck(
        long workId,
        CheckType checkType,
        bool isPassed,
        int attemptNumber = 1,
        int? expertId = null,
        decimal? resultValue = null,
        string? comment = null,
        string? documentPath = null)
    {
        WorkId = workId;
        CheckType = checkType;
        IsPassed = isPassed;
        AttemptNumber = attemptNumber;
        AssignedExpertId = expertId;
        ResultValue = resultValue;
        Comment = comment;
        DocumentPath = documentPath;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = expertId ?? 0;
    }

    /// <summary>
    /// Checks if this is an anti-plagiarism check with percentage result.
    /// </summary>
    public bool HasPercentageResult => CheckType == CheckType.AntiPlagiarism && ResultValue.HasValue;

    /// <summary>
    /// Gets the plagiarism percentage (for AntiPlagiarism checks).
    /// </summary>
    public decimal? GetPlagiarismPercentage()
    {
        return CheckType == CheckType.AntiPlagiarism ? ResultValue : null;
    }
}
