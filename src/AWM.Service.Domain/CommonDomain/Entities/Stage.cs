namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;

/// <summary>
/// Stage entity (formerly Period) for managing workflow stage time ranges.
/// Maps to [Common].[Stages].
/// </summary>
public class Stage : Entity<int>, IAuditable, ISoftDeletable
{
    public int OrgUnitId { get; private set; }
    public int SemesterId { get; private set; }
    public int WorkflowStageId { get; private set; }
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

    private Stage() { }

    public Stage(int orgUnitId, int semesterId, int workflowStageId, DateTime startDate, DateTime endDate, int createdBy)
    {
        if (endDate <= startDate)
            throw new DomainException("Stage.InvalidDateRange", "End date must be after start date.");

        OrgUnitId = orgUnitId;
        SemesterId = semesterId;
        WorkflowStageId = workflowStageId;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    public void UpdateDates(DateTime startDate, DateTime endDate, int modifiedBy)
    {
        if (endDate <= startDate)
            throw new DomainException("Stage.InvalidDateRange", "End date must be after start date.");

        StartDate = startDate;
        EndDate = endDate;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public bool IsCurrentlyOpen()
    {
        var now = DateTime.UtcNow;
        return IsActive && !IsDeleted && now >= StartDate && now <= EndDate;
    }

    public void Deactivate(int modifiedBy)
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void Activate(int modifiedBy)
    {
        IsActive = true;
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

    public DateRange GetDateRange()
    {
        return DateRange.Create(StartDate, EndDate);
    }
}
