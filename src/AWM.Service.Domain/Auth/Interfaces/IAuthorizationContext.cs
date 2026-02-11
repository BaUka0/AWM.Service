namespace AWM.Service.Domain.Auth.Interfaces;

using AWM.Service.Domain.Auth.Enums;

/// <summary>
/// Interface for accessing the current user's authorization context.
/// Provides role and permission checks with context awareness.
/// </summary>
public interface IAuthorizationContext
{
    /// <summary>
    /// Current user ID, or null if not authenticated.
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// Current user's university ID.
    /// </summary>
    int? UniversityId { get; }

    /// <summary>
    /// Current operation's department context, if applicable.
    /// </summary>
    int? CurrentDepartmentId { get; }

    /// <summary>
    /// Current operation's institute context, if applicable.
    /// </summary>
    int? CurrentInstituteId { get; }

    /// <summary>
    /// Current academic year context.
    /// </summary>
    int? CurrentAcademicYearId { get; }

    /// <summary>
    /// Whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// List of role names the user has (globally valid).
    /// </summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// Checks if user has a specific permission globally.
    /// </summary>
    bool HasPermission(Permission permission);

    /// <summary>
    /// Checks if user has a specific permission in the given department context.
    /// </summary>
    bool HasPermissionInDepartment(Permission permission, int departmentId);

    /// <summary>
    /// Checks if user has a specific permission in the given department and year context.
    /// </summary>
    bool HasPermissionInContext(Permission permission, int? departmentId, int? academicYearId);

    /// <summary>
    /// Checks if user has any of the specified permissions globally.
    /// </summary>
    bool HasAnyPermission(params Permission[] permissions);

    /// <summary>
    /// Checks if user has all of the specified permissions globally.
    /// </summary>
    bool HasAllPermissions(params Permission[] permissions);

    /// <summary>
    /// Gets all permissions the user has globally.
    /// </summary>
    IReadOnlySet<Permission> GetAllPermissions();

    /// <summary>
    /// Gets all permissions the user has in a specific department.
    /// </summary>
    IReadOnlySet<Permission> GetPermissionsInDepartment(int departmentId);
}
