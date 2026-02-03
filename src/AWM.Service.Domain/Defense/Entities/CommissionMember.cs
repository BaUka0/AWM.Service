namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;

/// <summary>
/// CommissionMember entity - member of a defense commission.
/// </summary>
public class CommissionMember : Entity<int>, IAuditable
{
    public int CommissionId { get; private set; }
    public int UserId { get; private set; }
    public RoleInCommission RoleInCommission { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private CommissionMember() { }

    internal CommissionMember(int commissionId, int userId, RoleInCommission roleInCommission, int createdBy = 0)
    {
        CommissionId = commissionId;
        UserId = userId;
        RoleInCommission = roleInCommission;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Updates the member's role.
    /// </summary>
    public void UpdateRole(RoleInCommission newRole, int modifiedBy)
    {
        RoleInCommission = newRole;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Checks if this member is the chairman.
    /// </summary>
    public bool IsChairman => RoleInCommission == RoleInCommission.Chairman;

    /// <summary>
    /// Checks if this member is the secretary.
    /// </summary>
    public bool IsSecretary => RoleInCommission == RoleInCommission.Secretary;
}
