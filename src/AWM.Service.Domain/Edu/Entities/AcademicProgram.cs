namespace AWM.Service.Domain.Edu.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Academic program entity - educational program within a department.
/// </summary>
public class AcademicProgram : Entity<int>, IAuditable, ISoftDeletable
{
    public int DepartmentId { get; private set; }
    public int DegreeLevelId { get; private set; }
    public string? Code { get; private set; }
    public string? Name { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private AcademicProgram() { }

    public AcademicProgram(int departmentId, int degreeLevelId, string name, int createdBy, string? code = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Program name is required.", nameof(name));

        DepartmentId = departmentId;
        DegreeLevelId = degreeLevelId;
        Name = name;
        Code = code;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    public void UpdateName(string name, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Program name is required.", nameof(name));

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
    /// Soft deletes the academic program.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
