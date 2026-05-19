namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University semester entity (read-only).
/// Maps to [Edu_Semesters] table in university database.
/// </summary>
public class Semester : Entity<int>
{
    public string Title { get; private set; } = null!;
    public DateTime StartsOn { get; private set; }
    public DateTime EndsOn { get; private set; }
    public int StudyYear { get; private set; }
    public int SemesterTypeId { get; private set; }

    // Navigation properties
    public SemesterType? SemesterType { get; private set; }

    private Semester() { }
}
