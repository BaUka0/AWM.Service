namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Grade entity - individual grade from a commission member.
/// </summary>
public class Grade : Entity<long>, IAuditable
{
    public long ScheduleId { get; private set; }
    public int MemberId { get; private set; }
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

    internal Grade(long scheduleId, int memberId, int criteriaId, int score, string? comment = null)
    {
        if (score < 0)
            throw new ArgumentException("Score cannot be negative.", nameof(score));

        ScheduleId = scheduleId;
        MemberId = memberId;
        CriteriaId = criteriaId;
        Score = score;
        Comment = comment;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = memberId; // Graded by member
    }

    /// <summary>
    /// Updates the grade score.
    /// </summary>
    public void UpdateScore(int score, int modifiedBy, string? comment = null)
    {
        if (score < 0)
            throw new ArgumentException("Score cannot be negative.", nameof(score));

        Score = score;
        Comment = comment;
        
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
