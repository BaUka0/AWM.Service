namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Protocol entity - official defense session protocol.
/// </summary>
public class Protocol : Entity<long>, IAuditable, ISoftDeletable
{
    public long ScheduleId { get; private set; }
    public int CommissionId { get; private set; }
    public DateTime SessionDate { get; private set; }
    public string? DocumentPath { get; private set; }
    public bool IsFinalized { get; private set; }
    public int? FinalizedBy { get; private set; }
    public DateTime? FinalizedAt { get; private set; }

    public decimal? FinalScoreNumeric { get; private set; }
    public string? FinalGradeLetter { get; private set; }
    public string? Decision { get; private set; }
    public int? DecisionType { get; private set; }
    public int? ReadinessPercent { get; private set; }
    public string? ProtocolNumber { get; private set; }
    public string? Comments { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Protocol() { }

    public Protocol(
        long scheduleId, 
        int commissionId, 
        DateTime sessionDate, 
        int createdBy,
        string? protocolNumber = null,
        decimal? finalScoreNumeric = null,
        string? finalGradeLetter = null,
        string? decision = null,
        string? comments = null,
        int? decisionType = null,
        int? readinessPercent = null)
    {
        ScheduleId = scheduleId;
        CommissionId = commissionId;
        SessionDate = sessionDate;
        ProtocolNumber = protocolNumber;
        FinalScoreNumeric = finalScoreNumeric;
        FinalGradeLetter = finalGradeLetter;
        Decision = decision;
        DecisionType = decisionType;
        ReadinessPercent = readinessPercent;
        Comments = comments;
        IsFinalized = false;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Attaches the protocol document.
    /// </summary>
    public void AttachDocument(string documentPath, int modifiedBy)
    {
        if (IsFinalized)
            throw new DomainException("Protocol.ModifyFinalized", "Cannot modify finalized protocol.");

        if (string.IsNullOrWhiteSpace(documentPath))
            throw new DomainException("Protocol.DocumentPathRequired", "Document path is required.");

        DocumentPath = documentPath;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Finalizes the protocol (no more changes allowed).
    /// </summary>
    public void Finalize(int finalizedBy)
    {
        if (IsFinalized)
            throw new DomainException("Protocol.AlreadyFinalized", "Protocol is already finalized.");

        IsFinalized = true;
        FinalizedBy = finalizedBy;
        FinalizedAt = DateTime.UtcNow;

        LastModifiedAt = FinalizedAt;
        LastModifiedBy = finalizedBy;
    }

    /// <summary>
    /// Updates final grading fields, protocol number, and decision/remarks.
    /// </summary>
    public void SetGradingAndDecision(
        decimal? finalScoreNumeric,
        string? finalGradeLetter,
        string? decision,
        string? protocolNumber,
        int modifiedBy,
        string? comments = null,
        int? decisionType = null,
        int? readinessPercent = null)
    {
        if (IsFinalized)
            throw new DomainException("Protocol.ModifyFinalized", "Cannot modify finalized protocol.");

        FinalScoreNumeric = finalScoreNumeric;
        FinalGradeLetter = finalGradeLetter;
        Decision = decision;
        DecisionType = decisionType;
        ReadinessPercent = readinessPercent;
        ProtocolNumber = protocolNumber;
        Comments = comments;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the protocol.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Checks if the protocol has a document attached.
    /// </summary>
    public bool HasDocument => !string.IsNullOrWhiteSpace(DocumentPath);
}
