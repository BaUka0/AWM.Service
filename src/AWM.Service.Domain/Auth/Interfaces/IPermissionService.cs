namespace AWM.Service.Domain.Auth.Interfaces;

using AWM.Service.Domain.Auth.Enums;

/// <summary>
/// Service for resolving permissions based on roles.
/// Maps roles to their granted permissions.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Gets all permissions granted to a specific role.
    /// </summary>
    IReadOnlySet<Permission> GetPermissionsForRole(string roleName);

    /// <summary>
    /// Gets all permissions for a user based on their active role assignments.
    /// </summary>
    Task<IReadOnlySet<Permission>> GetUserPermissionsAsync(
        int userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user permissions valid for a specific department and year context.
    /// Only includes permissions from role assignments matching the context.
    /// </summary>
    Task<IReadOnlySet<Permission>> GetUserPermissionsInContextAsync(
        int userId,
        int? departmentId,
        int? academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role grants a specific permission.
    /// </summary>
    bool RoleHasPermission(string roleName, Permission permission);
}
