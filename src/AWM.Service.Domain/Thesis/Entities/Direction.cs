namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;
using AWM.Service.Domain.Thesis.Events;
using AWM.Service.Domain.Wf.Entities;

/// <summary>
/// Direction entity - research direction for thesis topics.
/// Supervisors create directions, department approves them, then topics are created within approved directions.
/// </summary>
public class Direction : AggregateRoot<long>, IAuditable, ISoftDeletable
{
    public int DepartmentId { get; private set; }
    public int SupervisorId { get; private set; }
    public int AcademicYearId { get; private set; }
    public int WorkTypeId { get; private set; }

    public string TitleRu { get; private set; } = null!;
    public string? TitleEn { get; private set; }
    public string? TitleKz { get; private set; }
    public string? Description { get; private set; }

    public int CurrentStateId { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
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

    private readonly List<Topic> _topics = new();
    public IReadOnlyCollection<Topic> Topics => _topics.AsReadOnly();

    private Direction() { }

    public Direction(
        int departmentId,
        int supervisorId,
        int academicYearId,
        int workTypeId,
        string titleRu,
        int draftStateId,
        string? titleKz = null,
        string? titleEn = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new ArgumentException("Russian title is required.", nameof(titleRu));

        DepartmentId = departmentId;
        SupervisorId = supervisorId;
        AcademicYearId = academicYearId;
        WorkTypeId = workTypeId;
        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        Description = description;
        CurrentStateId = draftStateId;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = supervisorId;
        LastModifiedAt = CreatedAt;
        LastModifiedBy = supervisorId;
        IsDeleted = false;

        RaiseDomainEvent(new DirectionCreatedEvent(Id, supervisorId, departmentId));
    }

    /// <summary>
    /// Gets the multilingual title.
    /// </summary>
    public MultilingualText GetTitle()
    {
        return MultilingualText.Create(TitleRu, TitleKz, TitleEn);
    }

    /// <summary>
    /// Updates direction content (only in draft state).
    /// </summary>
    public void UpdateContent(string titleRu, string? titleKz, string? titleEn, string? description)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new ArgumentException("Russian title is required.", nameof(titleRu));

        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        Description = description;
    }

    /// <summary>
    /// Submits the direction for department review.
    /// </summary>
    public void Submit(int submittedStateId)
    {
        CurrentStateId = submittedStateId;
        SubmittedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DirectionSubmittedEvent(Id));
    }

    /// <summary>
    /// Approves the direction.
    /// </summary>
    public void Approve(int approvedStateId, int reviewedBy)
    {
        CurrentStateId = approvedStateId;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewComment = null;

        RaiseDomainEvent(new DirectionApprovedEvent(Id, reviewedBy));
    }

    /// <summary>
    /// Rejects the direction.
    /// </summary>
    public void Reject(int rejectedStateId, int reviewedBy, string? comment = null)
    {
        CurrentStateId = rejectedStateId;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewComment = comment;

        RaiseDomainEvent(new DirectionRejectedEvent(Id, reviewedBy, comment));
    }

    /// <summary>
    /// Requests revision of the direction.
    /// </summary>
    public void RequestRevision(int revisionStateId, int reviewedBy, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment is required for revision request.", nameof(comment));

        CurrentStateId = revisionStateId;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewComment = comment;

        RaiseDomainEvent(new DirectionRequiresRevisionEvent(Id, reviewedBy, comment));
    }

    /// <summary>
    /// Soft deletes the direction.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
