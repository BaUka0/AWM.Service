namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Expert entity - department staff member assigned for quality checks.
/// </summary>
public class Expert : Entity<int>, IAuditable, ISoftDeletable
{
    public int UserId { get; private set; }
    public int DepartmentId { get; private set; }
    public ExpertiseType ExpertiseType { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Expert() { }

    public Expert(int userId, int departmentId, ExpertiseType expertiseType, int createdBy = 0)
    {
        UserId = userId;
        DepartmentId = departmentId;
        ExpertiseType = expertiseType;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Activates the expert.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the expert.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft deletes the expert.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }
}
