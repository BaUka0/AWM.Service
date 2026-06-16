namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University employee entity (read-only).
/// Maps to [Edu_Employees] table in university database.
/// </summary>
public class Employee : Entity<int>
{
    public bool IsAdvisor { get; private set; }

    public User? User { get; private set; }
    public ICollection<EmployeePosition> Positions { get; private set; } = new List<EmployeePosition>();

    private Employee() { }
}
