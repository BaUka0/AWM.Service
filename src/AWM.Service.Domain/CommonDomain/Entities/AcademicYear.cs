namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;

/// <summary>
/// Academic year entity representing a study period (e.g., 2025-2026).
/// Contains archiving mechanism for read-only historical data.
/// </summary>
public class AcademicYear : Entity<int>, IAuditable, ISoftDeletable
{
    public int UniversityId { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public bool IsArchived { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private AcademicYear() { }

    public AcademicYear(int universityId, string name, DateTime startDate, DateTime endDate, int createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Academic year name is required.", nameof(name));
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.", nameof(endDate));

        UniversityId = universityId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        IsCurrent = false;
        IsArchived = false;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Sets this year as the current academic year.
    /// </summary>
    public void SetAsCurrent(int modifiedBy)
    {
        if (IsArchived)
            throw new InvalidOperationException("Cannot set archived year as current.");

        IsCurrent = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Removes current status from this year.
    /// </summary>
    public void UnsetCurrent(int modifiedBy)
    {
        IsCurrent = false;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Archives the academic year, making all associated data read-only.
    /// </summary>
    public void Archive(int modifiedBy)
    {
        if (IsCurrent)
            throw new InvalidOperationException("Cannot archive current academic year.");

        IsArchived = true;
        IsCurrent = false;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the academic year.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Gets the date range of this academic year.
    /// </summary>
    public DateRange GetDateRange()
    {
        return DateRange.Create(StartDate, EndDate);
    }
}
