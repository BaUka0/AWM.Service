namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University semester type entity (read-only).
/// Maps to [Edu_SemesterTypes] table in university database.
/// </summary>
public class SemesterType : Entity<int>
{
    public string Title { get; private set; } = null!;
    public int OrderBy { get; private set; }

    private SemesterType() { }
}
