namespace AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Status of a thesis topic in its lifecycle.
/// Replaces the legacy boolean flags (IsSubmittedForApproval, IsApproved, IsRejected, IsClosed).
/// </summary>
public enum TopicStatus
{
    /// <summary>Topic is in draft state, not yet submitted for review.</summary>
    Draft = 0,

    /// <summary>Topic has been submitted and awaiting department approval.</summary>
    Pending = 1,

    /// <summary>Topic has been approved by the department.</summary>
    Approved = 2,

    /// <summary>Topic has been rejected by the department.</summary>
    Rejected = 3,

    /// <summary>Topic is closed (no more applications accepted).</summary>
    Closed = 4
}
