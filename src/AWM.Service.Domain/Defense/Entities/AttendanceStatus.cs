namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Attendance status reference entity.
/// Maps to [Defense].[AttendanceStatuses].
/// </summary>
public class AttendanceStatus : Entity<int>
{
    public string Title { get; private set; } = null!;

    private AttendanceStatus() { }

    public AttendanceStatus(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("AttendanceStatus.TitleRequired", "Title is required.");

        Title = title;
    }
}
