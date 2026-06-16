namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University organizational unit type entity (read-only).
/// Maps to [Edu_OrgUnitTypes] table in university database.
/// </summary>
public class OrgUnitType : Entity<int>
{
    public string Title { get; private set; } = null!;

    private OrgUnitType() { }
}
