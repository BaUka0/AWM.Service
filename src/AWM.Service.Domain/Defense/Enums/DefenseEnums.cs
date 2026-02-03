namespace AWM.Service.Domain.Defense.Enums;

/// <summary>
/// Commission type for defense.
/// </summary>
public enum CommissionType
{
    /// <summary>
    /// Pre-defense commission.
    /// </summary>
    PreDefense,

    /// <summary>
    /// State Attestation Commission (ГАК).
    /// </summary>
    GAK
}

/// <summary>
/// Role within a commission.
/// </summary>
public enum RoleInCommission
{
    /// <summary>
    /// Commission chairman/president.
    /// </summary>
    Chairman,

    /// <summary>
    /// Technical secretary.
    /// </summary>
    Secretary,

    /// <summary>
    /// Regular member.
    /// </summary>
    Member
}

/// <summary>
/// Attendance status for pre-defense attempts.
/// </summary>
public enum AttendanceStatus
{
    /// <summary>
    /// Student attended.
    /// </summary>
    Attended,

    /// <summary>
    /// Student was absent without excuse.
    /// </summary>
    Absent,

    /// <summary>
    /// Student was absent with valid excuse.
    /// </summary>
    Excused
}
