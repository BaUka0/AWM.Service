namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Review entity - external review of a student work.
/// </summary>
public class Review : Entity<long>, IAuditable, ISoftDeletable
{
    public long WorkId { get; private set; }
    public int ReviewerId { get; private set; }
    public string? ReviewText { get; private set; }
    public string? FileStoragePath { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    // Legacy fields
    public int UploadedBy => CreatedBy;
    public DateTime? UploadedAt => LastModifiedAt;

    private Review() { }

    public Review(long workId, int reviewerId, int createdBy)
    {
        WorkId = workId;
        ReviewerId = reviewerId;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Uploads the review content.
    /// </summary>
    public void UploadReview(string? reviewText, string? fileStoragePath, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(reviewText) && string.IsNullOrWhiteSpace(fileStoragePath))
            throw new ArgumentException("Either review text or file path must be provided.");

        ReviewText = reviewText;
        FileStoragePath = fileStoragePath;
        
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the review.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Checks if the review has been uploaded.
    /// </summary>
    public bool IsUploaded => LastModifiedAt.HasValue;
}
