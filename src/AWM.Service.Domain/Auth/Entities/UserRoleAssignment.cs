namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// User role assignment entity - the key security table for Context-Aware RBAC.
/// Links User + Role + Context (Department, Institute, Year).
/// </summary>
public class UserRoleAssignment : Entity<long>, IAuditable
{
    public int UserId { get; private set; }
    public int RoleId { get; private set; }

    // Context fields
    public int? DepartmentId { get; private set; }
    public int? InstituteId { get; private set; }
    public int? AcademicYearId { get; private set; }

    // Validity period
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Legacy field
    public int? AssignedBy => CreatedBy;

    // Navigation property for authorization
    /// <summary>
    /// Navigation property to the Role entity.
    /// Used for eager loading in authorization queries.
    /// </summary>
    public Role? Role { get; private set; }

    private UserRoleAssignment() { }

    internal UserRoleAssignment(
        int userId,
        int roleId,
        int? departmentId = null,
        int? instituteId = null,
        int? academicYearId = null,
        int? createdBy = null)
    {
        UserId = userId;
        RoleId = roleId;
        DepartmentId = departmentId;
        InstituteId = instituteId;
        AcademicYearId = academicYearId;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? 0;
        ValidFrom = CreatedAt;
        ValidTo = null;
    }

    /// <summary>
    /// Checks if the assignment is currently valid.
    /// </summary>
    public bool IsCurrentlyValid()
    {
        var now = DateTime.UtcNow;
        return now >= ValidFrom && (ValidTo == null || now <= ValidTo);
    }

    /// <summary>
    /// Checks if the assignment applies to the given context.
    /// </summary>
    public bool AppliesToContext(int? departmentId, int? academicYearId)
    {
        if (!IsCurrentlyValid())
            return false;

        // If assignment has no context constraints, it applies
        if (DepartmentId == null && AcademicYearId == null)
            return true;

        // Check department match
        if (DepartmentId != null && DepartmentId != departmentId)
            return false;

        // Check year match
        if (AcademicYearId != null && AcademicYearId != academicYearId)
            return false;

        return true;
    }

    /// <summary>
    /// Revokes the role assignment.
    /// </summary>
    public void Revoke(int modifiedBy)
    {
        ValidTo = DateTime.UtcNow;
        LastModifiedAt = ValidTo;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Sets an expiration date for the assignment.
    /// </summary>
    public void SetExpiration(DateTime expirationDate, int modifiedBy)
    {
        if (expirationDate <= ValidFrom)
            throw new ArgumentException("Expiration date must be after validity start.", nameof(expirationDate));

        ValidTo = expirationDate;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
