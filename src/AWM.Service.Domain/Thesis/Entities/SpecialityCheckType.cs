namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.University;

/// <summary>
/// Junction entity defining which quality checks are required for a specific speciality.
/// All checks listed here must be passed before defense.
/// </summary>
public class SpecialityCheckType : Entity<int>
{
    public int OrgUnitId { get; private set; }
    public int? SpecialityId { get; private set; }
    public int CheckTypeId { get; private set; }
    public decimal? MinimumPassValue { get; private set; }
    public bool IsActive { get; private set; }

    public OrgUnit? OrgUnit { get; private set; }
    public Speciality? Speciality { get; private set; }
    public CheckType? CheckType { get; private set; }

    private SpecialityCheckType() { }

    public SpecialityCheckType(int orgUnitId, int checkTypeId, int? specialityId = null, decimal? minimumPassValue = null, bool isActive = true)
    {
        OrgUnitId = orgUnitId;
        CheckTypeId = checkTypeId;
        SpecialityId = specialityId;
        MinimumPassValue = minimumPassValue;
        IsActive = isActive;
    }

    public void Update(decimal? minimumPassValue, bool isActive)
    {
        MinimumPassValue = minimumPassValue;
        IsActive = isActive;
    }
}
