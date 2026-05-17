namespace AWM.Service.Domain.Org.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Institute/Faculty entity - structural unit within a University.
/// </summary>
public class Institute : Entity<int>, IAuditable, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public string? Code { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<Department> _departments = new();
    public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

    private Institute() { }

    internal Institute(string name, int createdBy = 0, string? code = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Institute name is required.", nameof(name));

        Name = name;
        Code = code;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    public void UpdateName(string name, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Institute name is required.", nameof(name));

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

    public Department AddDepartment(string name, int createdBy, string? code = null)
    {
        var department = new Department(this.Id, name, createdBy, code);
        _departments.Add(department);
        return department;
    }

    public static Institute Create(string name, int createdBy = 0, string? code = null)
    {
        return new Institute(name, createdBy, code);
    }

    /// <summary>
    /// Soft deletes the institute.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
