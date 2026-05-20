namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Thesis.Events;

/// <summary>
/// Topic entity - thesis topic proposed by supervisors.
/// Can be linked to a direction, supports team works (1-5 participants).
/// </summary>
public class Topic : AggregateRoot<long>, IAuditable, ISoftDeletable
{
    public long? DirectionId { get; private set; }
    public int SemesterId { get; private set; }
    public int OrgUnitId { get; private set; }
    public int EmployeeId { get; private set; }
    public int WorkTypeId { get; private set; }

    public string TitleRu { get; private set; } = null!;
    public string? TitleEn { get; private set; }
    public string? TitleKz { get; private set; }
    public string? DescriptionRu { get; private set; }
    public string? DescriptionKz { get; private set; }
    public string? DescriptionEn { get; private set; }

    public int MaxParticipants { get; private set; }
    public bool IsSubmittedForApproval { get; private set; }
    public bool IsApproved { get; private set; }
    public bool IsClosed { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<TopicApplication> _applications = new();
    public IReadOnlyCollection<TopicApplication> Applications => _applications.AsReadOnly();

    private Topic() { }

    public Topic(
        int orgUnitId,
        int employeeId,
        int semesterId,
        int workTypeId,
        string titleRu,
        long? directionId = null,
        string? titleKz = null,
        string? titleEn = null,
        string? descriptionRu = null,
        string? descriptionKz = null,
        string? descriptionEn = null,
        int maxParticipants = 1)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new DomainException("Topic.TitleRuRequired", "Russian title is required.");
        if (maxParticipants < 1 || maxParticipants > 3)
            throw new DomainException("Topic.MaxParticipantsOutOfRange", "Max participants must be between 1 and 3.");

        DirectionId = directionId;
        OrgUnitId = orgUnitId;
        EmployeeId = employeeId;
        SemesterId = semesterId;
        WorkTypeId = workTypeId;
        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        DescriptionRu = descriptionRu;
        DescriptionKz = descriptionKz;
        DescriptionEn = descriptionEn;
        MaxParticipants = maxParticipants;
        IsSubmittedForApproval = false;
        IsApproved = false;
        IsClosed = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = employeeId; // Topic creator is supervisor
        LastModifiedAt = CreatedAt;
        LastModifiedBy = employeeId;
        IsDeleted = false;

        RaiseDomainEvent(new TopicCreatedEvent(Id, directionId, employeeId));
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
        string? descriptionEn)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new DomainException("Topic.TitleRuRequired", "Russian title is required.");

        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        DescriptionRu = descriptionRu;
        DescriptionKz = descriptionKz;
        DescriptionEn = descriptionEn;
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
        if (IsSubmittedForApproval)
            throw new DomainException("Topic.AlreadySubmitted", "Topic is already submitted for approval.");
        if (IsApproved)
            throw new DomainException("Topic.AlreadyApproved", "Topic is already approved.");

        IsSubmittedForApproval = true;
    }

    /// <summary>
    /// Approves the topic for student selection.
    /// </summary>
    public void Approve()
    {
        IsApproved = true;
        RaiseDomainEvent(new TopicApprovedEvent(Id));
    }

    /// <summary>
    /// Revokes approval.
    /// </summary>
    public void RevokeApproval()
    {
        IsApproved = false;
    }

    /// <summary>
    /// Closes the topic (no more applications).
    /// </summary>
    public void Close()
    {
        IsClosed = true;
        RaiseDomainEvent(new TopicClosedEvent(Id));
    }
    
    /// <summary>
    /// Reopens the topic for applications.
    /// </summary>
    public void Reopen()
    {
        IsClosed = false;
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
        if (!IsApproved || IsClosed)
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
