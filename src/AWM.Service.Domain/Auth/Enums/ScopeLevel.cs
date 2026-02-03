namespace AWM.Service.Domain.Auth.Enums;

/// <summary>
/// Scope level determines where a role applies.
/// </summary>
public enum ScopeLevel
{
    /// <summary>
    /// Role applies globally across all tenants (super admin).
    /// </summary>
    Global,

    /// <summary>
    /// Role applies within a specific university.
    /// </summary>
    University,

    /// <summary>
    /// Role applies within a specific department.
    /// </summary>
    Department,

    /// <summary>
    /// Role applies within a specific commission.
    /// </summary>
    Commission
}
