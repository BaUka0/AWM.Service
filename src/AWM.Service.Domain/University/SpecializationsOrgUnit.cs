namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// Junction entity linking Specialization and OrgUnit.
/// Maps to [Edu_Specializations_OrgUnits] table in university database.
/// </summary>
public class SpecializationsOrgUnit : Entity<int>
{
    public int? SpecializationId { get; private set; }
    public int? OrgUnitId { get; private set; }

    public Specialization? Specialization { get; private set; }
    public OrgUnit? OrgUnit { get; private set; }

    private SpecializationsOrgUnit() { }
}
