namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Events;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// StudentWork entity - the main thesis work aggregate root.
/// Represents the actual work being done by student(s) on a topic.
/// </summary>
public class StudentWork : AggregateRoot<long>, IAuditable, ISoftDeletable
{
    public long? TopicId { get; private set; }
    public int AcademicYearId { get; private set; }
    public int DepartmentId { get; private set; }
    public int CurrentStateId { get; private set; }

    public string? FinalGrade { get; private set; }
    public bool IsDefended { get; private set; }
    public string? RepositoryUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<WorkParticipant> _participants = new();
    public IReadOnlyCollection<WorkParticipant> Participants => _participants.AsReadOnly();

    private readonly List<Attachment> _attachments = new();
    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

    private readonly List<QualityCheck> _qualityChecks = new();
    public IReadOnlyCollection<QualityCheck> QualityChecks => _qualityChecks.AsReadOnly();

    private readonly List<WorkflowHistory> _workflowHistory = new();
    public IReadOnlyCollection<WorkflowHistory> WorkflowHistory => _workflowHistory.AsReadOnly();

    private StudentWork() { }

    public StudentWork(
        int academicYearId,
        int departmentId,
        int draftStateId,
        int createdBy,
        long? topicId = null)
    {
        TopicId = topicId;
        AcademicYearId = academicYearId;
        DepartmentId = departmentId;
        CurrentStateId = draftStateId;
        IsDefended = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        LastModifiedAt = CreatedAt;
        LastModifiedBy = createdBy;
        IsDeleted = false;

        RaiseDomainEvent(new WorkCreatedEvent(Id, topicId, departmentId));
    }

    /// <summary>
    /// Adds a participant to the work.
    /// </summary>
    public WorkParticipant AddParticipant(int studentId, ParticipantRole role = ParticipantRole.Member)
    {
        // Check if already a participant
        if (_participants.Any(p => p.StudentId == studentId))
            throw new InvalidOperationException("Student is already a participant.");

        // Check max participants (5)
        if (_participants.Count >= 5)
            throw new InvalidOperationException("Maximum 5 participants allowed.");

        // Ensure only one leader
        if (role == ParticipantRole.Leader && _participants.Any(p => p.Role == ParticipantRole.Leader))
            throw new InvalidOperationException("Work already has a leader.");

        var participant = new WorkParticipant(Id, studentId, role);
        _participants.Add(participant);

        RaiseDomainEvent(new ParticipantJoinedEvent(Id, studentId, role.ToString()));
        return participant;
    }

    /// <summary>
    /// Removes a participant from the work.
    /// </summary>
    public void RemoveParticipant(int studentId)
    {
        var participant = _participants.FirstOrDefault(p => p.StudentId == studentId)
            ?? throw new InvalidOperationException("Student is not a participant of this work.");

        if (_participants.Count == 1)
            throw new InvalidOperationException("Cannot remove the last participant from the work.");

        if (participant.Role == ParticipantRole.Leader && _participants.Count > 1)
            throw new InvalidOperationException("Cannot remove the leader while other participants exist. Transfer leadership first.");

        _participants.Remove(participant);
    }

    /// <summary>
    /// Changes the work state.
    /// </summary>
    public void ChangeState(int newStateId, int changedBy, string? comment = null)
    {
        var oldStateId = CurrentStateId;
        CurrentStateId = newStateId;
        LastModifiedBy = changedBy;

        var historyEntry = new WorkflowHistory(Id, oldStateId, newStateId, changedBy, comment);
        _workflowHistory.Add(historyEntry);

        RaiseDomainEvent(new WorkStateChangedEvent(Id, oldStateId, newStateId, changedBy));
    }

    /// <summary>
    /// Adds an attachment to the work.
    /// </summary>
    public Attachment AddAttachment(
        AttachmentType attachmentType,
        string fileName,
        string fileStoragePath,
        string fileHash,
        int uploadedBy,
        int? stateId = null)
    {
        var attachment = new Attachment(
            Id,
            stateId ?? CurrentStateId,
            attachmentType,
            fileName,
            fileStoragePath,
            fileHash,
            uploadedBy);

        _attachments.Add(attachment);
        LastModifiedBy = uploadedBy;
        return attachment;
    }

    /// <summary>
    /// Removes an attachment from the work.
    /// </summary>
    public void RemoveAttachment(long attachmentId, int removedBy)
    {
        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId)
            ?? throw new InvalidOperationException($"Attachment with ID {attachmentId} was not found on this work.");

        _attachments.Remove(attachment);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = removedBy;
    }

    /// <summary>
    /// Submits the work for a quality check by creating a pending (unreviewed) record.
    /// The expert will later complete it via CompleteQualityCheck.
    /// </summary>
    public QualityCheck AddQualityCheck(
        CheckType checkType,
        bool isPassed,
        int? expertId = null,
        decimal? resultValue = null,
        string? comment = null,
        string? documentPath = null)
    {
        var attemptNumber = _qualityChecks.Count(c => c.CheckType == checkType) + 1;

        var check = new QualityCheck(
            Id,
            checkType,
            isPassed,
            attemptNumber,
            expertId,
            resultValue,
            comment,
            documentPath);

        _qualityChecks.Add(check);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = expertId ?? CreatedBy;

        return check;
    }

    /// <summary>
    /// Records an expert's result on an existing pending quality check.
    /// Finds the check by ID, validates it is still pending, updates it in-place,
    /// and raises the QualityCheckCompletedEvent domain event.
    /// </summary>
    public QualityCheck CompleteQualityCheck(
        long checkId,
        int expertId,
        bool isPassed,
        decimal? resultValue = null,
        string? comment = null,
        string? documentPath = null)
    {
        var check = _qualityChecks.FirstOrDefault(c => c.Id == checkId)
            ?? throw new InvalidOperationException(
                $"QualityCheck with ID {checkId} was not found on this work.");

        if (check.AssignedExpertId.HasValue)
            throw new InvalidOperationException(
                "This quality check result has already been recorded by an expert.");

        check.SetResult(expertId, isPassed, resultValue, comment, documentPath);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = expertId;

        RaiseDomainEvent(new QualityCheckCompletedEvent(Id, check.CheckType.ToString(), isPassed, expertId));

        return check;
    }

    /// <summary>
    /// Marks the work as defended with a final grade.
    /// </summary>
    public void MarkAsDefended(string? finalGrade)
    {
        IsDefended = true;
        FinalGrade = finalGrade;

        RaiseDomainEvent(new WorkDefendedEvent(Id, finalGrade));
    }

    /// <summary>
    /// Gets the leader of the work.
    /// </summary>
    public WorkParticipant? GetLeader()
    {
        return _participants.FirstOrDefault(p => p.Role == ParticipantRole.Leader);
    }

    /// <summary>
    /// Checks if a specific check type has passed.
    /// </summary>
    public bool HasPassedCheck(CheckType checkType)
    {
        return _qualityChecks.Any(c => c.CheckType == checkType && c.IsPassed);
    }

    /// <summary>
    /// Gets the latest check of a specific type.
    /// </summary>
    public QualityCheck? GetLatestCheck(CheckType checkType)
    {
        return _qualityChecks
            .Where(c => c.CheckType == checkType)
            .OrderByDescending(c => c.AttemptNumber)
            .FirstOrDefault();
    }
    /// <summary>
    /// Sets the repository URL for software check (e.g. GitHub link).
    /// </summary>
    public void SetRepositoryUrl(string repositoryUrl, int modifiedBy)
    {
        RepositoryUrl = repositoryUrl ?? throw new ArgumentNullException(nameof(repositoryUrl));
        LastModifiedBy = modifiedBy;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft deletes the work.
    /// </summary>
    public void Delete(int deletedBy)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restores a soft-deleted work.
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
