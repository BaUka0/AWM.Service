namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;

/// <summary>
/// Unified entity for staff role assignments (Supervisors, Commission Members, experts, etc.).
/// Provides a single point for managing and auditing staff responsibilities.
/// </summary>
public class StaffAssignment : AggregateRoot<long>, IAuditable, ISoftDeletable
{
    public int UserId { get; private set; }
    public StaffRoleType RoleType { get; private set; }

    /// <summary>
    /// Type of the entity this assignment is related to (e.g., "Commission", "StudentWork", "OrgUnit").
    /// </summary>
    public string TargetEntityType { get; private set; } = null!;

    /// <summary>
    /// ID of the entity this assignment is related to.
    /// </summary>
    public long TargetEntityId { get; private set; }

    /// <summary>
    /// Optional JSON metadata for role-specific context (e.g., CheckTypeId for QualityExpert).
    /// </summary>
    public string? MetadataJson { get; private set; }

    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private StaffAssignment() { }

    public StaffAssignment(
        int userId,
        StaffRoleType roleType,
        string targetEntityType,
        long targetEntityId,
        int createdBy,
        string? metadataJson = null,
        DateTime? validFrom = null)
    {
        UserId = userId;
        RoleType = roleType;
        TargetEntityType = targetEntityType;
        TargetEntityId = targetEntityId;
        MetadataJson = metadataJson;

        ValidFrom = validFrom ?? DateTime.UtcNow;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    public void Deactivate(int modifiedBy)
    {
        IsActive = false;
        ValidTo = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void Activate(int modifiedBy)
    {
        IsActive = true;
        ValidTo = null;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void UpdateMetadata(string? metadataJson, int modifiedBy)
    {
        MetadataJson = metadataJson;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    public void RaiseEmployeesApprovedEvent(int orgUnitId, int semesterId, int? specialityId, IReadOnlyList<int> employeeUserIds, int confirmedBy)
    {
        RaiseDomainEvent(new AWM.Service.Domain.CommonDomain.Events.EmployeesApprovedEvent(orgUnitId, semesterId, specialityId, employeeUserIds, confirmedBy));
    }
}
