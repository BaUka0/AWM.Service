namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University speciality level entity (read-only).
/// Maps to [Edu_SpecialityLevels] table in university database.
/// </summary>
public class SpecialityLevel : Entity<int>
{
    public string Title { get; private set; } = null!;

    private SpecialityLevel() { }
}
