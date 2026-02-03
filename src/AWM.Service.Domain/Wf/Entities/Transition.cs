namespace AWM.Service.Domain.Wf.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Transition entity - represents allowed state transitions in the workflow.
/// </summary>
public class Transition : Entity<int>, IAuditable, ISoftDeletable
{
    public int FromStateId { get; private set; }
    public int ToStateId { get; private set; }
    public int? AllowedRoleId { get; private set; }
    public bool IsAutomatic { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Transition() { }

    public Transition(int fromStateId, int toStateId, int createdBy = 0, int? allowedRoleId = null, bool isAutomatic = false)
    {
        FromStateId = fromStateId;
        ToStateId = toStateId;
        AllowedRoleId = allowedRoleId;
        IsAutomatic = isAutomatic;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Creates a manual transition requiring a specific role.
    /// </summary>
    public static Transition Manual(int fromStateId, int toStateId, int allowedRoleId, int createdBy = 0)
    {
        return new Transition(fromStateId, toStateId, createdBy, allowedRoleId, false);
    }

    /// <summary>
    /// Creates an automatic transition (triggered by system/events).
    /// </summary>
    public static Transition Automatic(int fromStateId, int toStateId, int createdBy = 0)
    {
        return new Transition(fromStateId, toStateId, createdBy, null, true);
    }

    /// <summary>
    /// Soft deletes the transition.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Checks if a user with the given role can perform this transition.
    /// </summary>
    public bool CanBePerformedBy(int roleId)
    {
        if (IsAutomatic || IsDeleted)
            return false;

        return AllowedRoleId == null || AllowedRoleId == roleId;
    }
}
