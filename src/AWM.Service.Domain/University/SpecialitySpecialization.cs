namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// Junction entity linking Speciality and Specialization.
/// Maps to [Edu_SpecialitySpecializations] table in university database.
/// </summary>
public class SpecialitySpecialization : Entity<int>
{
    public int? SpecialityId { get; private set; }
    public int? SpecializationId { get; private set; }

    // Navigation properties
    public Speciality? Speciality { get; private set; }
    public Specialization? Specialization { get; private set; }

    private SpecialitySpecialization() { }
}
