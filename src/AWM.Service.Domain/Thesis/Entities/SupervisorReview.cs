namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// SupervisorReview entity - supervisor's review/feedback on student work.
/// Required document for defense.
/// </summary>
public class SupervisorReview : Entity<long>, IAuditable, ISoftDeletable
{
    public long WorkId { get; private set; }
    public int SupervisorId { get; private set; }
    public string ReviewText { get; private set; } = null!;
    public string? FileStoragePath { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private SupervisorReview() { }

    public SupervisorReview(long workId, int supervisorId, string reviewText, int createdBy, string? fileStoragePath = null)
    {
        if (string.IsNullOrWhiteSpace(reviewText))
            throw new ArgumentException("Review text is required.", nameof(reviewText));

        WorkId = workId;
        SupervisorId = supervisorId;
        ReviewText = reviewText;
        FileStoragePath = fileStoragePath;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Updates the review.
    /// </summary>
    public void UpdateReview(string reviewText, string? fileStoragePath, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(reviewText))
            throw new ArgumentException("Review text is required.", nameof(reviewText));

        ReviewText = reviewText;
        FileStoragePath = fileStoragePath;

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the supervisor review.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Checks if signed file is attached.
    /// </summary>
    public bool HasSignedFile => !string.IsNullOrWhiteSpace(FileStoragePath);
}
