namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// WorkParticipant entity - represents a student participating in a work.
/// Supports team works with Leader and Member roles.
/// </summary>
public class WorkParticipant : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int StudentId { get; private set; }
    public int RoleId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Seeded reference IDs
    private const int RoleLeader = 1;
    private const int RoleMember = 2;

    // Legacy field
    public DateTime JoinedAt => CreatedAt;

    private WorkParticipant() { }

    internal WorkParticipant(long workId, int studentId, int roleId, int createdBy = 0)
    {
        WorkId = workId;
        StudentId = studentId;
        RoleId = roleId;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Promotes member to leader.
    /// </summary>
    public void PromoteToLeader(int modifiedBy)
    {
        if (RoleId == RoleLeader)
            throw new DomainException("WorkParticipant.AlreadyLeader", "Already a leader.");

        RoleId = RoleLeader;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Demotes leader to member.
    /// </summary>
    public void DemoteToMember(int modifiedBy)
    {
        if (RoleId == RoleMember)
            throw new DomainException("WorkParticipant.AlreadyMember", "Already a member.");

        RoleId = RoleMember;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Checks if this participant is the team leader.
    /// </summary>
    public bool IsLeader => RoleId == RoleLeader;
}
