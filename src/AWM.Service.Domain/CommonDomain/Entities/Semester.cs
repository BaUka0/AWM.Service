namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Primitives;

/// <summary>
/// Semester entity representing a specific study semester.
/// Maps to [Edu].[Semesters].
/// </summary>
public class Semester : Entity<int>, IAuditable, ISoftDeletable
{
    public int SemesterTypeId { get; private set; }
    public string Title { get; private set; } = null!;
    public DateTime StartsOn { get; private set; }
    public DateTime EndsOn { get; private set; }
    public int StudyYear { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private Semester() { }

    public Semester(int semesterTypeId, string title, DateTime startsOn, DateTime endsOn, int studyYear, int createdBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (endsOn <= startsOn)
            throw new ArgumentException("End date must be after start date.", nameof(endsOn));

        SemesterTypeId = semesterTypeId;
        Title = title;
        StartsOn = startsOn;
        EndsOn = endsOn;
        StudyYear = studyYear;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    public void UpdateDates(DateTime startsOn, DateTime endsOn, int modifiedBy)
    {
        if (endsOn <= startsOn)
            throw new ArgumentException("End date must be after start date.", nameof(endsOn));

        StartsOn = startsOn;
        EndsOn = endsOn;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void UpdateTitle(string title, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void UpdateStudyYear(int studyYear, int modifiedBy)
    {
        StudyYear = studyYear;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
