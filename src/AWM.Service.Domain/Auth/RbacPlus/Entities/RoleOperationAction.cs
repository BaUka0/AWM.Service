namespace AWM.Service.Domain.Auth.RbacPlus.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Permission matrix: Role x Operation x ActionType.
/// </summary>
public class RoleOperationAction : Entity<int>
{
    public int RoleAccessId { get; private set; }
    public int RoleOperationId { get; private set; }
    public int RoleActionTypeId { get; private set; }

    public RoleAccess RoleAccess { get; private set; } = null!;
    public RoleOperation RoleOperation { get; private set; } = null!;
    public RoleActionType RoleActionType { get; private set; } = null!;

    private RoleOperationAction() { }

    public RoleOperationAction(int roleAccessId, int roleOperationId, int roleActionTypeId)
    {
        RoleAccessId = roleAccessId;
        RoleOperationId = roleOperationId;
        RoleActionTypeId = roleActionTypeId;
    }
}
