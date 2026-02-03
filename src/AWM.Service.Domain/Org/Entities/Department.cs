namespace AWM.Service.Domain.Org.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Department entity - the core functional unit where thesis work management occurs.
/// Most roles and processes are scoped to the department level.
/// </summary>
public class Department : Entity<int>, IAuditable, ISoftDeletable
{
    public int InstituteId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Code { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Department() { }

    internal Department(int instituteId, string name, int createdBy = 0, string? code = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required.", nameof(name));

        InstituteId = instituteId;
        Name = name;
        Code = code;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    public void UpdateName(string name, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required.", nameof(name));

        Name = name;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void UpdateCode(string? code, int modifiedBy)
    {
        Code = code;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the department.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
