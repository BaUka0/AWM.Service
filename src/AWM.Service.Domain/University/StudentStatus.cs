namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University student status entity (read-only).
/// Maps to [Edu_StudentStatuses] table in university database.
/// </summary>
public class StudentStatus : Entity<int>
{
    public string Title { get; private set; } = null!;

    private StudentStatus() { }
}
