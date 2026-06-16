namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University position entity (read-only).
/// Maps to [Edu_Positions] table in university database.
/// </summary>
public class Position : Entity<int>
{
    public string Title { get; private set; } = null!;
    public bool Deleted { get; private set; }

    private Position() { }
}
