namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Audit history for user role access changes.
/// </summary>
public class UserAccessHistory : Entity<int>, IAuditable
{
    public int UserId { get; private set; }
    public int RoleAccessId { get; private set; }
    public string Action { get; private set; } = null!; // Added / Removed
    public int? AssignedBy { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private UserAccessHistory() { }

    public UserAccessHistory(int userId, int roleAccessId, string action, int? assignedBy = null)
    {
        if (userId <= 0)
            throw new DomainException("UserAccessHistory.InvalidUserId", "UserId must be positive.");
        if (roleAccessId <= 0)
            throw new DomainException("UserAccessHistory.InvalidRoleAccessId", "RoleAccessId must be positive.");
        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("UserAccessHistory.ActionRequired", "Action is required.");

        UserId = userId;
        RoleAccessId = roleAccessId;
        Action = action;
        AssignedBy = assignedBy;
        AssignedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = assignedBy ?? 0;
    }
}
