namespace AWM.Service.Domain.Edu.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Edu.Enums;

/// <summary>
/// Student entity - educational profile of a student.
/// </summary>
public class Student : AggregateRoot<int>, IAuditable, ISoftDeletable
{
    public int UserId { get; private set; }
    public int ProgramId { get; private set; }
    public int AdmissionYear { get; private set; }
    public int CurrentCourse { get; private set; }
    public string? GroupCode { get; private set; }
    public StudentStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Student() { }

    public Student(int userId, int programId, int admissionYear, int currentCourse, int createdBy, string? groupCode = null)
    {
        if (currentCourse <= 0)
            throw new ArgumentException("Current course must be positive.", nameof(currentCourse));

        UserId = userId;
        ProgramId = programId;
        AdmissionYear = admissionYear;
        CurrentCourse = currentCourse;
        GroupCode = groupCode;
        Status = StudentStatus.Active;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Promotes student to the next course.
    /// </summary>
    public void PromoteToCourse(int course, int modifiedBy)
    {
        if (course <= CurrentCourse)
            throw new ArgumentException("New course must be higher than current.", nameof(course));
        if (Status != StudentStatus.Active)
            throw new InvalidOperationException("Only active students can be promoted.");

        CurrentCourse = course;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Graduates the student.
    /// </summary>
    public void Graduate(int modifiedBy)
    {
        if (Status != StudentStatus.Active)
            throw new InvalidOperationException("Only active students can graduate.");

        Status = StudentStatus.Graduated;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Puts student on academic leave.
    /// </summary>
    public void TakeLeave(int modifiedBy)
    {
        if (Status != StudentStatus.Active)
            throw new InvalidOperationException("Only active students can take leave.");

        Status = StudentStatus.OnLeave;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Returns student from leave.
    /// </summary>
    public void ReturnFromLeave(int modifiedBy)
    {
        if (Status != StudentStatus.OnLeave)
            throw new InvalidOperationException("Only students on leave can return.");

        Status = StudentStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Expels the student.
    /// </summary>
    public void Expel(int modifiedBy)
    {
        Status = StudentStatus.Expelled;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Updates the group code.
    /// </summary>
    public void UpdateGroup(string groupCode, int modifiedBy)
    {
        GroupCode = groupCode;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public bool IsEligibleForDefense(int programDurationYears)
    {
        return Status == StudentStatus.Active && CurrentCourse >= programDurationYears;
    }

    /// <summary>
    /// Soft deletes the student profile.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        Status = StudentStatus.Expelled;
    }
}
