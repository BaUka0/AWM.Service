namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.University;

/// <summary>
/// Links a user to a role access.
/// </summary>
public class UserAccess : Entity<int>, IAuditable
{
    public int UserId { get; private set; }
    public int RoleAccessId { get; private set; }
    public int? AssignedBy { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public User User { get; private set; } = null!;
    public RoleAccess RoleAccess { get; private set; } = null!;

    private UserAccess() { }

    public UserAccess(int userId, int roleAccessId, int? assignedBy = null)
    {
        if (userId <= 0)
            throw new DomainException("UserAccess.InvalidUserId", "UserId must be positive.");
        if (roleAccessId <= 0)
            throw new DomainException("UserAccess.InvalidRoleAccessId", "RoleAccessId must be positive.");

        UserId = userId;
        RoleAccessId = roleAccessId;
        AssignedBy = assignedBy;
        AssignedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = assignedBy ?? 0;
    }
}
