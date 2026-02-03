namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Topic application entity - student's application to a topic.
/// </summary>
public class TopicApplication : Entity<long>, IAuditable, ISoftDeletable
{
    public long TopicId { get; private set; }
    public int StudentId { get; private set; }
    public string? MotivationLetter { get; private set; }
    public DateTime AppliedAt { get; private set; }
    public ApplicationStatus Status { get; private set; }
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

    private TopicApplication() { }

    public TopicApplication(long topicId, int studentId, string? motivationLetter = null)
    {
        TopicId = topicId;
        StudentId = studentId;
        MotivationLetter = motivationLetter;
        AppliedAt = DateTime.UtcNow;
        Status = ApplicationStatus.Submitted;

        CreatedAt = AppliedAt;
        CreatedBy = studentId; // Student is the creator
        IsDeleted = false;
    }

    /// <summary>
    /// Accepts the application.
    /// </summary>
    public void Accept(int reviewedBy)
    {
        if (Status != ApplicationStatus.Submitted)
            throw new InvalidOperationException("Only submitted applications can be accepted.");

        Status = ApplicationStatus.Accepted;
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
        if (Status != ApplicationStatus.Submitted)
            throw new InvalidOperationException("Only submitted applications can be rejected.");

        Status = ApplicationStatus.Rejected;
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
    public bool IsPending => Status == ApplicationStatus.Submitted;

    /// <summary>
    /// Checks if the application was accepted.
    /// </summary>
    public bool IsAccepted => Status == ApplicationStatus.Accepted;
}
