namespace AWM.Service.Domain.Edu.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Staff entity - profile of a university employee (supervisor, expert, etc.).
/// </summary>
public class Staff : AggregateRoot<int>, IAuditable, ISoftDeletable
{
    public int UserId { get; private set; }
    public int DepartmentId { get; private set; }
    public string? Position { get; private set; }
    public string? AcademicDegree { get; private set; }
    public int MaxStudentsLoad { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Staff() { }

    public Staff(int userId, int departmentId, int createdBy, string? position = null, string? academicDegree = null, int maxStudentsLoad = 5)
    {
        if (maxStudentsLoad <= 0)
            throw new ArgumentException("Max students load must be positive.", nameof(maxStudentsLoad));

        UserId = userId;
        DepartmentId = departmentId;
        Position = position;
        AcademicDegree = academicDegree;
        MaxStudentsLoad = maxStudentsLoad;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Soft deletes the staff profile.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Updates staff position.
    /// </summary>
    public void UpdatePosition(string? position, int modifiedBy)
    {
        Position = position;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Updates academic degree.
    /// </summary>
    public void UpdateAcademicDegree(string? academicDegree, int modifiedBy)
    {
        AcademicDegree = academicDegree;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Updates the maximum number of students this staff can supervise.
    /// </summary>
    public void UpdateMaxStudentsLoad(int maxLoad, int modifiedBy)
    {
        if (maxLoad <= 0)
            throw new ArgumentException("Max students load must be positive.", nameof(maxLoad));

        MaxStudentsLoad = maxLoad;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Checks if staff can take more students based on current load.
    /// </summary>
    public bool CanTakeMoreStudents(int currentStudentsCount)
    {
        return currentStudentsCount < MaxStudentsLoad;
    }

    /// <summary>
    /// Transfers staff to another department.
    /// </summary>
    public void TransferToDepartment(int newDepartmentId, int modifiedBy)
    {
        DepartmentId = newDepartmentId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
