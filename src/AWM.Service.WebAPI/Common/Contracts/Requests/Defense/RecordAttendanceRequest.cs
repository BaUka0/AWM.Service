namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for recording a student's attendance at a pre-defense.
/// </summary>
public sealed record RecordAttendanceRequest
{
    /// <summary>
    /// Attendance status (1 = Attended, 2 = Absent, 3 = Excused).
    /// </summary>
    /// <example>1</example>
    public int AttendanceStatus { get; init; }

    /// <summary>
    /// Whether the absence is excused. Only relevant when AttendanceStatus is Absent.
    /// </summary>
    /// <example>false</example>
    public bool IsExcused { get; init; }
}
