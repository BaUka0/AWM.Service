namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Reviewer entity - external reviewer (not a university employee).
/// </summary>
public class Reviewer : AggregateRoot<int>, IAuditable, ISoftDeletable
{
    public string FullName { get; private set; } = null!;
    public string? Position { get; private set; }
    public string? AcademicDegree { get; private set; }
    public string? Organization { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Reviewer() { }

    public Reviewer(
        string fullName,
        int createdBy,
        string? position = null,
        string? academicDegree = null,
        string? organization = null,
        string? email = null,
        string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        FullName = fullName;
        Position = position;
        AcademicDegree = academicDegree;
        Organization = organization;
        Email = email;
        Phone = phone;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Updates reviewer information.
    /// </summary>
    public void UpdateInfo(
        string fullName,
        int modifiedBy,
        string? position,
        string? academicDegree,
        string? organization,
        string? email,
        string? phone)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        FullName = fullName;
        Position = position;
        AcademicDegree = academicDegree;
        Organization = organization;
        Email = email;
        Phone = phone;

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Activates the reviewer.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the reviewer.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft deletes the reviewer.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }
}
