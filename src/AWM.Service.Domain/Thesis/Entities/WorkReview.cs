namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// WorkReview entity - universal entity for reviews (Supervisor, External, etc.).
/// Files are stored in the Attachment table.
/// </summary>
public class WorkReview : Entity<long>, IAuditable, ISoftDeletable
{
    public long WorkId { get; private set; }
    public int AuthorUserId { get; private set; }
    public ReviewType Type { get; private set; }
    public string ReviewText { get; private set; } = null!;
    public string? MetadataJson { get; private set; }
    public bool IsFinal { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private WorkReview() { }

    public WorkReview(long workId, int authorUserId, ReviewType type, string reviewText, int createdBy, string? metadataJson = null)
    {
        if (string.IsNullOrWhiteSpace(reviewText))
            throw new DomainException("WorkReview.ReviewTextRequired", "Review text is required.");

        WorkId = workId;
        AuthorUserId = authorUserId;
        Type = type;
        ReviewText = reviewText;
        MetadataJson = metadataJson;
        IsFinal = false;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Updates the review text and metadata.
    /// </summary>
    public void UpdateReview(string reviewText, string? metadataJson, int modifiedBy)
    {
        if (IsFinal)
            throw new DomainException("WorkReview.AlreadyFinal", "Cannot update a finalized review.");

        if (string.IsNullOrWhiteSpace(reviewText))
            throw new DomainException("WorkReview.ReviewTextRequired", "Review text is required.");

        ReviewText = reviewText;
        MetadataJson = metadataJson;

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Marks the review as final, preventing further text edits.
    /// </summary>
    public void FinalizeReview(int modifiedBy)
    {
        IsFinal = true;
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
}
