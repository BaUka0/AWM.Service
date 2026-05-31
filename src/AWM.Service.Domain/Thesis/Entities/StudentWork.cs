namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Thesis.Events;


/// <summary>
/// StudentWork entity - the main thesis work aggregate root.
/// Represents the actual work being done by student(s) on a topic.
/// </summary>
public class StudentWork : AggregateRoot<long>, IAuditable, ISoftDeletable
{
    public long? TopicId { get; private set; }
    public int SemesterId { get; private set; }
    public int OrgUnitId { get; private set; }
    public int? SpecialityId { get; private set; }
    public int CurrentStateId { get; private set; }

    public string? FinalGrade { get; private set; }
    public bool IsDefended { get; private set; }
    public string? MetadataJson { get; private set; }
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

    private readonly List<WorkReview> _workReviews = new();
    public IReadOnlyCollection<WorkReview> WorkReviews => _workReviews.AsReadOnly();

    private StudentWork() { }

    public StudentWork(
        int semesterId,
        int orgUnitId,
        int draftStateId,
        int createdBy,
        long? topicId = null,
        int? specialityId = null)
    {
        TopicId = topicId;
        SemesterId = semesterId;
        OrgUnitId = orgUnitId;
        SpecialityId = specialityId;
        CurrentStateId = draftStateId;
        IsDefended = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        LastModifiedAt = CreatedAt;
        LastModifiedBy = createdBy;
        IsDeleted = false;

        // NOTE: Domain event is NOT raised in constructor because EF Identity
        // has not yet assigned the Id. Call RaiseCreatedEvent() after AddAsync/SaveChanges.
    }

    /// <summary>
    /// Raises the WorkCreatedEvent. Must be called after the entity is persisted
    /// and has a valid Id assigned by the database.
    /// </summary>
    public void RaiseCreatedEvent()
    {
        RaiseDomainEvent(new WorkCreatedEvent(Id, TopicId, OrgUnitId));
    }

    /// <summary>
    /// Adds a participant to the work.
    /// </summary>
    /// <param name="studentId">The student's user ID.</param>
    /// <param name="maxParticipants">Maximum allowed participants (from related Topic).</param>
    public WorkParticipant AddParticipant(int studentId, int maxParticipants)
    {
        // Check if already a participant
        if (_participants.Any(p => p.StudentId == studentId))
            throw new DomainException("StudentWork.AlreadyParticipant", "Student is already a participant.");

        // Check max participants from the associated topic
        if (_participants.Count >= maxParticipants)
            throw new DomainException("StudentWork.MaxParticipantsExceeded", $"Maximum {maxParticipants} participants allowed.");

        var participant = new WorkParticipant(Id, studentId);
        _participants.Add(participant);

        RaiseDomainEvent(new ParticipantJoinedEvent(Id, studentId));
        return participant;
    }

    /// <summary>
    /// Removes a participant from the work.
    /// </summary>
    public void RemoveParticipant(int studentId)
    {
        var participant = _participants.FirstOrDefault(p => p.StudentId == studentId)
            ?? throw new DomainException("StudentWork.NotParticipant", "Student is not a participant of this work.");

        if (_participants.Count == 1)
            throw new DomainException("StudentWork.CannotRemoveLastParticipant", "Cannot remove the last participant from the work.");

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
        int attachmentTypeId,
        string fileName,
        string fileStoragePath,
        string fileHash,
        int uploadedBy,
        long fileSizeBytes,
        string contentType,
        int? stateId = null)
    {
        var attachment = new Attachment(
            Id,
            stateId ?? CurrentStateId,
            attachmentTypeId,
            fileName,
            fileStoragePath,
            fileHash,
            uploadedBy,
            fileSizeBytes,
            contentType);

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
            ?? throw new DomainException("StudentWork.AttachmentNotFound", $"Attachment with ID {attachmentId} was not found on this work.");

        _attachments.Remove(attachment);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = removedBy;
    }

    /// <summary>
    /// Submits the work for a quality check by creating a pending (unreviewed) record.
    /// The expert will later complete it via CompleteQualityCheck.
    /// </summary>
    public QualityCheck AddQualityCheck(
        int checkTypeId,
        bool isPassed,
        int? expertId = null,
        decimal? resultValue = null,
        string? comment = null,
        long? attachmentId = null)
    {
        var attemptNumber = _qualityChecks.Count(c => c.CheckTypeId == checkTypeId) + 1;

        var check = new QualityCheck(
            Id,
            checkTypeId,
            isPassed,
            attemptNumber,
            expertId,
            resultValue,
            comment,
            attachmentId);

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
        long checkId, int expertId, bool isPassed,
        decimal? resultValue = null, string? comment = null, long? attachmentId = null)
    {
        var check = _qualityChecks.FirstOrDefault(c => c.Id == checkId)
            ?? throw new DomainException("StudentWork.QualityCheckNotFound",
                $"QualityCheck with ID {checkId} was not found on this work.");

        if (check.AssignedExpertId.HasValue)
            throw new DomainException("StudentWork.QualityCheckAlreadyRecorded",
                "This quality check result has already been recorded by an expert.");

        check.SetResult(expertId, isPassed, resultValue, comment, attachmentId);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = expertId;

        RaiseDomainEvent(new QualityCheckCompletedEvent(Id, check.CheckTypeId.ToString(), isPassed, expertId));

        return check;
    }

    /// <summary>
    /// Updates the attachment linked to a specific quality check.
    /// </summary>
    public void UpdateCheckAttachment(long checkId, long attachmentId, int modifiedBy)
    {
        var check = _qualityChecks.FirstOrDefault(c => c.Id == checkId)
            ?? throw new DomainException("StudentWork.QualityCheckNotFound", 
                $"QualityCheck with ID {checkId} was not found on this work.");
        check.UpdateAttachmentId(attachmentId, modifiedBy);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
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
    /// Marks the work as graduated (completed university).
    /// </summary>
    public void MarkAsGraduated(string? finalGrade)
    {
        IsDefended = true;
        FinalGrade = finalGrade; // Allow updating grade
        // Optionally add WorkGraduatedEvent if needed
    }

    /// <summary>
    /// Checks if the work is eligible for defense by verifying all mandatory checks are passed.
    /// </summary>
    /// <param name="mandatoryCheckTypeIds">List of check type IDs required for the student's speciality.</param>
    public bool IsEligibleForDefense(IEnumerable<int> mandatoryCheckTypeIds)
    {
        return mandatoryCheckTypeIds.All(HasPassedCheck);
    }

    /// <summary>
    /// Checks if a specific check type has passed.
    /// </summary>
    public bool HasPassedCheck(int checkTypeId)
    {
        return _qualityChecks.Any(c => c.CheckTypeId == checkTypeId && c.IsPassed);
    }

    /// <summary>
    /// Gets the latest check of a specific type.
    /// </summary>
    public QualityCheck? GetLatestCheck(int checkTypeId)
    {
        return _qualityChecks
            .Where(c => c.CheckTypeId == checkTypeId)
            .OrderByDescending(c => c.AttemptNumber)
            .FirstOrDefault();
    }

    /// <summary>
    /// Updates the metadata JSON string for dynamic properties (e.g. GitHub link).
    /// </summary>
    public void UpdateMetadata(string? metadataJson, int modifiedBy)
    {
        MetadataJson = metadataJson;
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

    /// <summary>
    /// Adds a work review (Supervisor, External, etc.).
    /// </summary>
    public WorkReview AddReview(int authorUserId, ReviewType type, string reviewText, int createdBy, string? metadataJson = null)
    {
        var review = new WorkReview(Id, authorUserId, type, reviewText, createdBy, metadataJson);
        _workReviews.Add(review);
        LastModifiedBy = createdBy;
        LastModifiedAt = DateTime.UtcNow;
        return review;
    }

    /// <summary>
    /// Removes a work review.
    /// </summary>
    public void RemoveReview(long reviewId, int removedBy)
    {
        var review = _workReviews.FirstOrDefault(r => r.Id == reviewId)
            ?? throw new DomainException("StudentWork.ReviewNotFound", $"Review with ID {reviewId} was not found on this work.");

        _workReviews.Remove(review);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = removedBy;
    }
}
