namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University speciality entity (read-only).
/// Maps to [Edu_Specialities] table in university database.
/// </summary>
public class Speciality : Entity<int>
{
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? ShortTitle { get; private set; }
    public int YearsOfStudy { get; private set; }
    public int LevelId { get; private set; }
    public bool Deleted { get; private set; }

    // Navigation properties
    public SpecialityLevel? Level { get; private set; }

    private Speciality() { }
}
