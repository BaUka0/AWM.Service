namespace AWM.Service.Application.Authorization;

using AWM.Service.Domain.Auth.Enums;

/// <summary>
/// Represents a user's role assignment with its context and permissions.
/// </summary>
public sealed record RoleAssignmentContext(
    int AssignmentId,
    string RoleName,
    int? DepartmentId,
    int? InstituteId,
    int? AcademicYearId,
    DateTime ValidFrom,
    DateTime? ValidTo,
    IReadOnlySet<Permission> Permissions)
{
    /// <summary>
    /// Checks if this assignment is currently valid.
    /// </summary>
    public bool IsCurrentlyValid()
    {
        var now = DateTime.UtcNow;
        return now >= ValidFrom && (ValidTo == null || now <= ValidTo);
    }

    /// <summary>
    /// Checks if this assignment applies to the given context.
    /// </summary>
    public bool AppliesToContext(int? departmentId, int? academicYearId)
    {
        if (!IsCurrentlyValid())
            return false;

        // Global assignment (no context restrictions)
        if (DepartmentId == null && AcademicYearId == null)
            return true;

        // Check department match
        if (DepartmentId != null && DepartmentId != departmentId)
            return false;

        // Check year match
        if (AcademicYearId != null && AcademicYearId != academicYearId)
            return false;

        return true;
    }
}

/// <summary>
/// Complete user context including all role assignments and permissions.
/// </summary>
public sealed record UserContext(
    int UserId,
    int UniversityId,
    int? StudentId,
    int? StaffId,
    IReadOnlyList<RoleAssignmentContext> RoleAssignments)
{
    private HashSet<Permission>? _cachedGlobalPermissions;

    /// <summary>
    /// Gets all globally valid permissions (from assignments without department/year restrictions).
    /// </summary>
    public IReadOnlySet<Permission> GetGlobalPermissions()
    {
        if (_cachedGlobalPermissions != null)
            return _cachedGlobalPermissions;

        _cachedGlobalPermissions = RoleAssignments
            .Where(ra => ra.IsCurrentlyValid() && ra.DepartmentId == null)
            .SelectMany(ra => ra.Permissions)
            .ToHashSet();

        return _cachedGlobalPermissions;
    }

    /// <summary>
    /// Gets all permissions valid in the specified department.
    /// Includes global permissions and department-specific permissions.
    /// </summary>
    public IReadOnlySet<Permission> GetPermissionsInDepartment(int departmentId)
    {
        var permissions = new HashSet<Permission>(GetGlobalPermissions());

        var departmentPermissions = RoleAssignments
            .Where(ra => ra.IsCurrentlyValid() && ra.DepartmentId == departmentId)
            .SelectMany(ra => ra.Permissions);

        foreach (var permission in departmentPermissions)
        {
            permissions.Add(permission);
        }

        return permissions;
    }

    /// <summary>
    /// Gets all permissions valid in the specified context (department + year).
    /// </summary>
    public IReadOnlySet<Permission> GetPermissionsInContext(int? departmentId, int? academicYearId)
    {
        var permissions = new HashSet<Permission>();

        foreach (var assignment in RoleAssignments)
        {
            if (assignment.AppliesToContext(departmentId, academicYearId))
            {
                foreach (var permission in assignment.Permissions)
                {
                    permissions.Add(permission);
                }
            }
        }

        return permissions;
    }

    /// <summary>
    /// Checks if user has a specific permission globally.
    /// </summary>
    public bool HasPermission(Permission permission)
        => GetGlobalPermissions().Contains(permission);

    /// <summary>
    /// Checks if user has a specific permission in the given department.
    /// </summary>
    public bool HasPermissionInDepartment(Permission permission, int departmentId)
        => GetPermissionsInDepartment(departmentId).Contains(permission);

    /// <summary>
    /// Checks if user has a specific permission in the given context.
    /// </summary>
    public bool HasPermissionInContext(Permission permission, int? departmentId, int? academicYearId)
        => GetPermissionsInContext(departmentId, academicYearId).Contains(permission);

    /// <summary>
    /// Gets all role names the user has.
    /// </summary>
    public IReadOnlyList<string> GetRoleNames()
        => RoleAssignments
            .Where(ra => ra.IsCurrentlyValid())
            .Select(ra => ra.RoleName)
            .Distinct()
            .ToList();
}
