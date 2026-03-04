namespace AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Application status for topic applications.
/// </summary>
public enum ApplicationStatus
{
    /// <summary>
    /// Application submitted by student, waiting for supervisor decision.
    /// </summary>
    Submitted,

    /// <summary>
    /// Application accepted by supervisor.
    /// </summary>
    Accepted,

    /// <summary>
    /// Application rejected by supervisor.
    /// </summary>
    Rejected
}

/// <summary>
/// Participant role in a team work.
/// </summary>
public enum ParticipantRole
{
    /// <summary>
    /// Team leader (primary author).
    /// </summary>
    Leader,

    /// <summary>
    /// Team member.
    /// </summary>
    Member
}

/// <summary>
/// Type of file attachment.
/// </summary>
public enum AttachmentType
{
    /// <summary>
    /// Draft version of the work.
    /// </summary>
    Draft,

    /// <summary>
    /// Final version of the work.
    /// </summary>
    Final,

    /// <summary>
    /// Presentation slides.
    /// </summary>
    Presentation,

    /// <summary>
    /// Software/code archive.
    /// </summary>
    Software,

    /// <summary>
    /// Demo materials for defense.
    /// </summary>
    Demo,

    /// <summary>
    /// Handout materials for defense commission.
    /// </summary>
    Handout
}

/// <summary>
/// Type of expertise for quality checks.
/// </summary>
public enum ExpertiseType
{
    /// <summary>
    /// Formatting/standards check.
    /// </summary>
    NormControl,

    /// <summary>
    /// Software/code review.
    /// </summary>
    SoftwareCheck,

    /// <summary>
    /// Plagiarism check.
    /// </summary>
    AntiPlagiarism
}

/// <summary>
/// Type of quality check.
/// </summary>
public enum CheckType
{
    /// <summary>
    /// Formatting/standards check (norms compliance).
    /// </summary>
    NormControl,

    /// <summary>
    /// Software/code quality review.
    /// </summary>
    SoftwareCheck,

    /// <summary>
    /// Plagiarism detection check.
    /// </summary>
    AntiPlagiarism
}
