namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University employee position entity (read-only).
/// Maps to [Edu_EmployeePositions] table in university database.
/// </summary>
public class EmployeePosition : Entity<int>
{
    public int EmployeeId { get; private set; }
    public int OrgUnitId { get; private set; }
    public int PositionId { get; private set; }
    public DateTime? StartedOn { get; private set; }
    public DateTime? EndedOn { get; private set; }
    public decimal? Rate { get; private set; }
    public bool IsMainPosition { get; private set; }

    // Navigation properties
    public Employee? Employee { get; private set; }
    public OrgUnit? OrgUnit { get; private set; }
    public Position? Position { get; private set; }

    private EmployeePosition() { }
}
