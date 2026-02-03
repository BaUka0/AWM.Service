namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Primitives;

/// <summary>
/// Period entity for managing workflow stage time ranges.
/// Blocks actions outside of configured periods.
/// </summary>
public class Period : Entity<int>, IAuditable, ISoftDeletable
{
    public int DepartmentId { get; private set; }
    public int AcademicYearId { get; private set; }
    public WorkflowStage WorkflowStage { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Period() { }

    public Period(
        int departmentId,
        int academicYearId,
        WorkflowStage workflowStage,
        DateTime startDate,
        DateTime endDate,
        int createdBy)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.", nameof(endDate));

        DepartmentId = departmentId;
        AcademicYearId = academicYearId;
        WorkflowStage = workflowStage;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Updates the period dates.
    /// </summary>
    public void UpdateDates(DateTime startDate, DateTime endDate, int modifiedBy)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.", nameof(endDate));

        StartDate = startDate;
        EndDate = endDate;
        
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Checks if the current date is within this period.
    /// </summary>
    public bool IsCurrentlyOpen()
    {
        var now = DateTime.UtcNow;
        return IsActive && !IsDeleted && now >= StartDate && now <= EndDate;
    }

    /// <summary>
    /// Deactivates the period.
    /// </summary>
    public void Deactivate(int modifiedBy)
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Activates the period.
    /// </summary>
    public void Activate(int modifiedBy)
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the period.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    /// <summary>
    /// Gets the date range of this period.
    /// </summary>
    public DateRange GetDateRange()
    {
        return DateRange.Create(StartDate, EndDate);
    }
}
