namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Grade entity - individual grade from a commission member.
/// </summary>
public class Grade : Entity<long>, IAuditable
{
    public long ScheduleId { get; private set; }
    public long AssignmentId { get; private set; }
    public int CriteriaId { get; private set; }
    public int Score { get; private set; }
    public string? Comment { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Legacy field
    public DateTime GradedAt => CreatedAt;

    private Grade() { }

    internal Grade(long scheduleId, long assignmentId, int criteriaId, int score, int createdBy, string? comment = null)
    {
        if (score < 0)
            throw new DomainException("Grade.InvalidScore", "Score cannot be negative.");

        ScheduleId = scheduleId;
        AssignmentId = assignmentId;
        CriteriaId = criteriaId;
        Score = score;
        Comment = comment;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Updates the grade score.
    /// </summary>
    public void UpdateScore(int score, int modifiedBy, string? comment = null)
    {
        if (score < 0)
            throw new DomainException("Grade.InvalidScore", "Score cannot be negative.");

        Score = score;
        Comment = comment;

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
