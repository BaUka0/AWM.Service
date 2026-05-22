namespace AWM.Service.Domain.University;

using AWM.Service.Domain.Common;

/// <summary>
/// University user entity (read-only).
/// Maps to [Edu_Users] table in university database.
/// </summary>
public class User : Entity<int>
{
    public string LastName { get; private set; } = null!;
    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? IIN { get; private set; }
    public string? Email { get; private set; }
    public DateTime? DOB { get; private set; }
    public bool? Male { get; private set; }
    public string? MobilePhone { get; private set; }
    public string? PhotoFileName { get; private set; }

    private User() { }
}
