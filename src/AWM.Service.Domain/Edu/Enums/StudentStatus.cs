namespace AWM.Service.Domain.Edu.Enums;

/// <summary>
/// Student status in the educational system.
/// </summary>
public enum StudentStatus
{
    /// <summary>
    /// Currently enrolled and studying.
    /// </summary>
    Active,

    /// <summary>
    /// Successfully graduated.
    /// </summary>
    Graduated,

    /// <summary>
    /// Academic leave.
    /// </summary>
    OnLeave,

    /// <summary>
    /// Expelled from the university.
    /// </summary>
    Expelled,

    /// <summary>
    /// Transferred to another program/university.
    /// </summary>
    Transferred
}
