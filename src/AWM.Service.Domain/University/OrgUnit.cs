namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University organizational unit entity (read-only).
/// Maps to [Edu_OrgUnits] table in university database.
/// TypeId=1 → Department, TypeId=2 → Institute.
/// </summary>
public class OrgUnit : Entity<int>
{
    public int? ParentId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? ShortTitle { get; private set; }
    public int TypeId { get; private set; }
    public bool Deleted { get; private set; }

    // Navigation properties
    public OrgUnitType? Type { get; private set; }
    public OrgUnit? Parent { get; private set; }
    public ICollection<OrgUnit> Children { get; private set; } = new List<OrgUnit>();

    private OrgUnit() { }
}
