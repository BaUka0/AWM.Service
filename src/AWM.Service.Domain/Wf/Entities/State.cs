namespace AWM.Service.Domain.Wf.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// State entity - represents a state in the workflow state machine.
/// </summary>
public class State : Entity<int>, IAuditable, ISoftDeletable
{
    public int WorkTypeId { get; private set; }
    public string SystemName { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public bool IsFinal { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private State() { }

    public State(int workTypeId, string systemName, int createdBy = 0, string? displayName = null, bool isFinal = false)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            throw new ArgumentException("System name is required.", nameof(systemName));

        WorkTypeId = workTypeId;
        SystemName = systemName;
        DisplayName = displayName ?? systemName;
        IsFinal = isFinal;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks this state as a final state.
    /// </summary>
    public void MarkAsFinal(int modifiedBy)
    {
        IsFinal = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Updates the display name.
    /// </summary>
    public void UpdateDisplayName(string displayName, int modifiedBy)
    {
        DisplayName = displayName;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the state.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}

/// <summary>
/// Well-known state names for Direction workflow.
/// </summary>
public static class DirectionStates
{
    public const string Draft = "DirectionDraft";
    public const string Submitted = "DirectionSubmitted";
    public const string Approved = "DirectionApproved";
    public const string Rejected = "DirectionRejected";
    public const string RequiresRevision = "DirectionRequiresRevision";
}

/// <summary>
/// Well-known state names for StudentWork workflow.
/// </summary>
public static class WorkStates
{
    public const string Draft = "Draft";
    public const string OnReview = "OnReview";
    public const string NormControl = "NormControl";
    public const string SoftwareCheck = "SoftwareCheck";
    public const string AntiPlagiarism = "AntiPlagiarism";
    public const string PreDefense1 = "PreDefense1";
    public const string PreDefense2 = "PreDefense2";
    public const string PreDefense3 = "PreDefense3";
    public const string ReadyForDefense = "ReadyForDefense";
    public const string Defended = "Defended";
    public const string Cancelled = "Cancelled";
}
