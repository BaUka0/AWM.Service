namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University specialization entity (read-only).
/// Maps to [Edu_Specializations] table in university database.
/// </summary>
public class Specialization : Entity<int>
{
    public string? TitleRu { get; private set; }
    public string? TitleKz { get; private set; }
    public string? TitleEn { get; private set; }
    public string? Code { get; private set; }

    private Specialization() { }
}
