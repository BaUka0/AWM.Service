namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

using AWM.Service.Domain.Defense.Enums;

/// <summary>
/// Request contract for recording a student's attendance at a pre-defense.
/// </summary>
public sealed record RecordAttendanceRequest
{
    /// <summary>
    /// Attendance status (0 = Attended, 1 = Absent, 2 = Excused).
    /// </summary>
    /// <example>0</example>
    public AttendanceStatus AttendanceStatus { get; init; }

    /// <summary>
    /// Whether the absence is excused. Only relevant when AttendanceStatus is Absent.
    /// </summary>
    /// <example>false</example>
    public bool IsExcused { get; init; }
}
