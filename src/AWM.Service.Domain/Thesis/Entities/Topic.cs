namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Thesis.Events;

/// <summary>
/// Topic entity - thesis topic proposed by supervisors.
/// Can be linked to a direction, supports team works (1-3 participants).
/// Uses <see cref="TopicStatus"/> enum instead of separate boolean flags.
/// </summary>
public class Topic : AggregateRoot<long>, IAuditable, ISoftDeletable
{
    public long? DirectionId { get; private set; }
    public int SemesterId { get; private set; }
    public int OrgUnitId { get; private set; }
    public int WorkTypeId { get; private set; }
    public int? SpecialityId { get; private set; }

    public string TitleRu { get; private set; } = null!;
    public string? TitleEn { get; private set; }
    public string? TitleKz { get; private set; }
    public string? DescriptionRu { get; private set; }
    public string? DescriptionKz { get; private set; }
    public string? DescriptionEn { get; private set; }

    public int MaxParticipants { get; private set; }

    /// <summary>
    /// Current topic status. Replaces the legacy boolean flags
    /// (IsSubmittedForApproval, IsApproved, IsRejected, IsClosed).
    /// </summary>
    public TopicStatus Status { get; private set; }

    public string? ReviewComment { get; private set; }
    public int? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    // Navigation properties
    public University.Speciality? Speciality { get; private set; }

    private readonly List<TopicApplication> _applications = new();
    public IReadOnlyCollection<TopicApplication> Applications => _applications.AsReadOnly();

    private Topic() { }

    public Topic(
        int orgUnitId,
        int createdByUserId,
        int semesterId,
        int workTypeId,
        string titleRu,
        long? directionId = null,
        string? titleKz = null,
        string? titleEn = null,
        string? descriptionRu = null,
        string? descriptionKz = null,
        string? descriptionEn = null,
        int maxParticipants = 1,
        int? specialityId = null)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new DomainException("Topic.TitleRuRequired", "Russian title is required.");
        if (maxParticipants < 1 || maxParticipants > 3)
            throw new DomainException("Topic.MaxParticipantsOutOfRange", "Max participants must be between 1 and 3.");

        DirectionId = directionId;
        OrgUnitId = orgUnitId;
        SemesterId = semesterId;
        WorkTypeId = workTypeId;
        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        DescriptionRu = descriptionRu;
        DescriptionKz = descriptionKz;
        DescriptionEn = descriptionEn;
        MaxParticipants = maxParticipants;
        SpecialityId = specialityId;
        Status = TopicStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdByUserId; // Topic creator is supervisor
        LastModifiedAt = CreatedAt;
        LastModifiedBy = createdByUserId;
        IsDeleted = false;

        // NOTE: Domain event is NOT raised in constructor because EF Identity
        // has not yet assigned the Id. Call RaiseCreatedEvent() after AddAsync/SaveChanges.
    }

    /// <summary>
    /// Raises the TopicCreatedEvent. Must be called after the entity is persisted
    /// and has a valid Id assigned by the database.
    /// </summary>
    public void RaiseCreatedEvent()
    {
        RaiseDomainEvent(new TopicCreatedEvent(Id, DirectionId, CreatedBy));
    }

    /// <summary>
    /// Gets the multilingual title.
    /// </summary>
    public MultilingualText GetTitle()
    {
        return MultilingualText.Create(TitleRu, TitleKz, TitleEn);
    }

    /// <summary>
    /// Updates topic content.
    /// </summary>
    public void UpdateContent(
        string titleRu,
        string? titleKz,
        string? titleEn,
        string? descriptionRu,
        string? descriptionKz,
        string? descriptionEn,
        int? maxParticipants = null)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new DomainException("Topic.TitleRuRequired", "Russian title is required.");

        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        DescriptionRu = descriptionRu;
        DescriptionKz = descriptionKz;
        DescriptionEn = descriptionEn;

        if (maxParticipants.HasValue)
        {
            UpdateMaxParticipants(maxParticipants.Value);
        }
    }

    /// <summary>
    /// Updates max participants for team works.
    /// </summary>
    public void UpdateMaxParticipants(int maxParticipants)
    {
        if (maxParticipants < 1 || maxParticipants > 3)
            throw new DomainException("Topic.MaxParticipantsOutOfRange", "Max participants must be between 1 and 3.");

        MaxParticipants = maxParticipants;
    }

    /// <summary>
    /// Marks the topic as submitted for department approval.
    /// </summary>
    public void SubmitForApproval()
    {
        if (Status == TopicStatus.Approved)
            throw new DomainException("Topic.AlreadyApproved", "Topic is already approved.");

        Status = TopicStatus.Pending;
        ReviewComment = null;
    }

    /// <summary>
    /// Approves the topic for student selection.
    /// </summary>
    public void Approve(int reviewedBy)
    {
        Status = TopicStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewComment = null;

        RaiseDomainEvent(new TopicApprovedEvent(Id));
    }

    /// <summary>
    /// Rejects the topic.
    /// </summary>
    public void Reject(int reviewedBy, string comment)
    {
        Status = TopicStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewComment = comment;
    }

    /// <summary>
    /// Revokes approval, returning topic to Pending status.
    /// </summary>
    public void RevokeApproval()
    {
        Status = TopicStatus.Pending;
    }

    /// <summary>
    /// Closes the topic (no more applications).
    /// </summary>
    public void Close()
    {
        Status = TopicStatus.Closed;
        RaiseDomainEvent(new TopicClosedEvent(Id));
    }
    
    /// <summary>
    /// Reopens the topic for applications.
    /// </summary>
    public void Reopen()
    {
        Status = TopicStatus.Approved;
    }
    
    public void AddApplication(TopicApplication application)
    {
        if (application is null)
            throw new DomainException("Topic.ApplicationRequired", "Application is required.");
    
        _applications.Add(application);
    }

    /// <summary>
    /// Checks if the topic can accept more applications.
    /// </summary>
    public bool CanAcceptApplications()
    {
        if (Status != TopicStatus.Approved)
            return false;

        var acceptedCount = _applications.Count(a => a.StatusId == (int)ApplicationStatusType.Accepted);
        return acceptedCount < MaxParticipants;
    }

    /// <summary>
    /// Gets the number of available spots.
    /// </summary>
    public int GetAvailableSpots()
    {
        var acceptedCount = _applications.Count(a => a.StatusId == (int)ApplicationStatusType.Accepted);
        return Math.Max(0, MaxParticipants - acceptedCount);
    }

    /// <summary>
    /// Soft deletes the topic.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Checks if this is a team topic.
    /// </summary>
    public bool IsTeamTopic => MaxParticipants > 1;
}
