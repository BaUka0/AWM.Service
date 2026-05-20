namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.University;

/// <summary>
/// Junction entity defining which quality checks are required for a specific speciality.
/// All checks listed here must be passed before defense.
/// </summary>
public class SpecialityCheckType : Entity<int>
{
    public int SpecialityId { get; private set; }
    public int CheckTypeId { get; private set; }

    // Navigation properties
    public Speciality? Speciality { get; private set; }
    public CheckType? CheckType { get; private set; }

    private SpecialityCheckType() { }

    public SpecialityCheckType(int specialityId, int checkTypeId)
    {
        SpecialityId = specialityId;
        CheckTypeId = checkTypeId;
    }
}
