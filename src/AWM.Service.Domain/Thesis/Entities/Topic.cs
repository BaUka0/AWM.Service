namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;
using AWM.Service.Domain.Thesis.Events;

/// <summary>
/// Topic entity - thesis topic proposed by supervisors.
/// Can be linked to a direction, supports team works (1-5 participants).
/// </summary>
public class Topic : AggregateRoot<long>, IAuditable, ISoftDeletable
{
    public long? DirectionId { get; private set; }
    public int AcademicYearId { get; private set; }
    public int DepartmentId { get; private set; }
    public int SupervisorId { get; private set; }
    public int WorkTypeId { get; private set; }

    public string TitleRu { get; private set; } = null!;
    public string? TitleEn { get; private set; }
    public string? TitleKz { get; private set; }
    public string? Description { get; private set; }

    public int MaxParticipants { get; private set; }
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
        int departmentId,
        int supervisorId,
        int academicYearId,
        int workTypeId,
        string titleRu,
        long? directionId = null,
        string? titleKz = null,
        string? titleEn = null,
        string? description = null,
        int maxParticipants = 1)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new ArgumentException("Russian title is required.", nameof(titleRu));
        if (maxParticipants < 1 || maxParticipants > 5)
            throw new ArgumentException("Max participants must be between 1 and 5.", nameof(maxParticipants));

        DirectionId = directionId;
        DepartmentId = departmentId;
        SupervisorId = supervisorId;
        AcademicYearId = academicYearId;
        WorkTypeId = workTypeId;
        TitleRu = titleRu;
        TitleKz = titleKz;
        TitleEn = titleEn;
        Description = description;
        MaxParticipants = maxParticipants;
        IsApproved = false;
        IsClosed = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = supervisorId; // Topic creator is supervisor
        LastModifiedAt = CreatedAt;
        LastModifiedBy = supervisorId;
        IsDeleted = false;

        RaiseDomainEvent(new TopicCreatedEvent(Id, directionId, supervisorId));
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
    /// Updates max participants for team works.
    /// </summary>
    public void UpdateMaxParticipants(int maxParticipants)
    {
        if (maxParticipants < 1 || maxParticipants > 5)
            throw new ArgumentException("Max participants must be between 1 and 5.", nameof(maxParticipants));

        MaxParticipants = maxParticipants;
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
            throw new ArgumentNullException(nameof(application));
    
        _applications.Add(application);
    }

    /// <summary>
    /// Checks if the topic can accept more applications.
    /// </summary>
    public bool CanAcceptApplications()
    {
        if (!IsApproved || IsClosed)
            return false;

        var acceptedCount = _applications.Count(a => a.Status == Enums.ApplicationStatus.Accepted);
        return acceptedCount < MaxParticipants;
    }

    /// <summary>
    /// Gets the number of available spots.
    /// </summary>
    public int GetAvailableSpots()
    {
        var acceptedCount = _applications.Count(a => a.Status == Enums.ApplicationStatus.Accepted);
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
