namespace AWM.Service.Domain.Edu.Entities;

using AWM.Service.Domain.Common;

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
    public int StatusId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    // Seeded reference IDs
    private const int StatusActive = 1;
    private const int StatusGraduated = 2;
    private const int StatusOnLeave = 3;
    private const int StatusExpelled = 4;

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
        StatusId = StatusActive;
        
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
        if (StatusId != StatusActive)
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
        if (StatusId != StatusActive)
            throw new InvalidOperationException("Only active students can graduate.");

        StatusId = StatusGraduated;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Puts student on academic leave.
    /// </summary>
    public void TakeLeave(int modifiedBy)
    {
        if (StatusId != StatusActive)
            throw new InvalidOperationException("Only active students can take leave.");

        StatusId = StatusOnLeave;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Returns student from leave.
    /// </summary>
    public void ReturnFromLeave(int modifiedBy)
    {
        if (StatusId != StatusOnLeave)
            throw new InvalidOperationException("Only students on leave can return.");

        StatusId = StatusActive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Expels the student.
    /// </summary>
    public void Expel(int modifiedBy)
    {
        StatusId = StatusExpelled;
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
        return StatusId == StatusActive && CurrentCourse >= programDurationYears;
    }

    /// <summary>
    /// Soft deletes the student profile.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        StatusId = StatusExpelled;
    }
}
