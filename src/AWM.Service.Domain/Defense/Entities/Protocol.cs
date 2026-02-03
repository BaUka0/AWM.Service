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

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Protocol() { }

    public Protocol(long scheduleId, int commissionId, DateTime sessionDate, int createdBy)
    {
        ScheduleId = scheduleId;
        CommissionId = commissionId;
        SessionDate = sessionDate;
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
            throw new InvalidOperationException("Cannot modify finalized protocol.");

        if (string.IsNullOrWhiteSpace(documentPath))
            throw new ArgumentException("Document path is required.", nameof(documentPath));

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
            throw new InvalidOperationException("Protocol is already finalized.");

        IsFinalized = true;
        FinalizedBy = finalizedBy;
        FinalizedAt = DateTime.UtcNow;

        LastModifiedAt = FinalizedAt;
        LastModifiedBy = finalizedBy;
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
