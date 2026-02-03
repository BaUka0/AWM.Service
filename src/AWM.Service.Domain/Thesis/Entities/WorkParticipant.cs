namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// WorkParticipant entity - represents a student participating in a work.
/// Supports team works with Leader and Member roles.
/// </summary>
public class WorkParticipant : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int StudentId { get; private set; }
    public ParticipantRole Role { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Legacy field
    public DateTime JoinedAt => CreatedAt;

    private WorkParticipant() { }

    internal WorkParticipant(long workId, int studentId, ParticipantRole role, int createdBy = 0)
    {
        WorkId = workId;
        StudentId = studentId;
        Role = role;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Promotes member to leader.
    /// </summary>
    public void PromoteToLeader(int modifiedBy)
    {
        if (Role == ParticipantRole.Leader)
            throw new InvalidOperationException("Already a leader.");

        Role = ParticipantRole.Leader;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Demotes leader to member.
    /// </summary>
    public void DemoteToMember(int modifiedBy)
    {
        if (Role == ParticipantRole.Member)
            throw new InvalidOperationException("Already a member.");

        Role = ParticipantRole.Member;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Checks if this participant is the team leader.
    /// </summary>
    public bool IsLeader => Role == ParticipantRole.Leader;
}
