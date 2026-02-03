namespace AWM.Service.Domain.Auth.Enums;

/// <summary>
/// System role types for RBAC.
/// </summary>
public enum RoleType
{
    /// <summary>
    /// Student role - can select topics, submit works, etc.
    /// </summary>
    Student,

    /// <summary>
    /// Supervisor/Scientific advisor role - manages directions, topics, advises students.
    /// </summary>
    Supervisor,

    /// <summary>
    /// Head of Department role - approves directions, manages department workflow.
    /// </summary>
    HeadOfDepartment,

    /// <summary>
    /// Secretary role - manages schedules, prints reports.
    /// </summary>
    Secretary,

    /// <summary>
    /// Expert role - performs quality checks (NormControl, Software, AntiPlagiarism).
    /// </summary>
    Expert,

    /// <summary>
    /// System administrator role - full access.
    /// </summary>
    Admin,

    /// <summary>
    /// Commission member role - evaluates defenses.
    /// </summary>
    CommissionMember,

    /// <summary>
    /// Vice-Rector for Academic Affairs - has access to all statistics.
    /// </summary>
    ViceRector
}
