namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University student entity (read-only).
/// Maps to [Edu_Students] table in university database.
/// </summary>
public class Student : Entity<int>
{
    public int? SpecialityId { get; private set; }
    public int? StatusId { get; private set; }
    public int? CategoryId { get; private set; }
    public int Year { get; private set; }
    public double? GPA { get; private set; }
    public double? EctsGPA { get; private set; }
    public int? EducationTypeId { get; private set; }
    public int? GrantTypeId { get; private set; }
    public int? AdvisorId { get; private set; }
    public int? StudyLanguageId { get; private set; }
    public int? AcademicStatusId { get; private set; }
    public bool? IsScholarship { get; private set; }
    public bool NeedsDorm { get; private set; }
    public DateTime? EntryDate { get; private set; }

    // Navigation properties
    public User? User { get; private set; }
    public Speciality? Speciality { get; private set; }
    public StudentStatus? Status { get; private set; }

    private Student() { }
}
