namespace AWM.Service.Domain.Org.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// University entity - the root tenant in the multi-tenant architecture.
/// All data in the system belongs to a specific university.
/// </summary>
public class University : AggregateRoot<int>, IAuditable, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<Institute> _institutes = new();
    public IReadOnlyCollection<Institute> Institutes => _institutes.AsReadOnly();

    private University() { }

    public University(string name, string code)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("University name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("University code is required.", nameof(code));
        if (code.Length > 50)
            throw new ArgumentException("University code must be 50 characters or less.", nameof(code));

        Name = name;
        Code = code.ToUpperInvariant();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = 0;
        LastModifiedAt = CreatedAt;
        LastModifiedBy = 0;
        IsDeleted = false;
    }

    /// <summary>
    /// Soft deletes the university.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void UpdateName(string name, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("University name is required.", nameof(name));

        Name = name;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public Institute AddInstitute(string name, int createdBy)
    {
        var institute = new Institute(this.Id, name, createdBy);
        _institutes.Add(institute);
        return institute;
    }
}
