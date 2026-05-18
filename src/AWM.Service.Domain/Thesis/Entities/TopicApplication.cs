namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Topic application entity - student's application to a topic.
/// </summary>
public class TopicApplication : Entity<long>, IAuditable, ISoftDeletable
{
    public long TopicId { get; private set; }
    public int StudentId { get; private set; }
    public string? MotivationLetter { get; private set; }
    public DateTime AppliedAt { get; private set; }
    public int StatusId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public int? ReviewedBy { get; private set; }
    public string? ReviewComment { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    // Seeded reference IDs
    private const int StatusSubmitted = 1;
    private const int StatusAccepted = 2;
    private const int StatusRejected = 3;

    private TopicApplication() { }

    public TopicApplication(long topicId, int studentId, string? motivationLetter = null)
    {
        TopicId = topicId;
        StudentId = studentId;
        MotivationLetter = motivationLetter;
        AppliedAt = DateTime.UtcNow;
        StatusId = StatusSubmitted;

        CreatedAt = AppliedAt;
        CreatedBy = studentId; // Student is the creator
        IsDeleted = false;
    }

    /// <summary>
    /// Accepts the application.
    /// </summary>
    public void Accept(int reviewedBy)
    {
        if (StatusId != StatusSubmitted)
            throw new InvalidOperationException("Only submitted applications can be accepted.");

        StatusId = StatusAccepted;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewComment = null;
        
        LastModifiedAt = ReviewedAt;
        LastModifiedBy = reviewedBy;
    }

    /// <summary>
    /// Rejects the application.
    /// </summary>
    public void Reject(int reviewedBy, string? comment = null)
    {
        if (StatusId != StatusSubmitted)
            throw new InvalidOperationException("Only submitted applications can be rejected.");

        StatusId = StatusRejected;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewComment = comment;

        LastModifiedAt = ReviewedAt;
        LastModifiedBy = reviewedBy;
    }

    /// <summary>
    /// Soft deletes the application.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Checks if the application is pending review.
    /// </summary>
    public bool IsPending => StatusId == StatusSubmitted;

    /// <summary>
    /// Checks if the application was accepted.
    /// </summary>
    public bool IsAccepted => StatusId == StatusAccepted;
}
