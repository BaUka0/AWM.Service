namespace AWM.Service.WebAPI.Authorization;

/// <summary>
/// Constants for authorization claim types.
/// </summary>
public static class AuthorizationConstants
{
    /// <summary>
    /// Claim type for global permissions (applies everywhere).
    /// </summary>
    public const string PermissionClaimType = "permission";

    /// <summary>
    /// Claim type for permissions that apply globally (no department restriction).
    /// </summary>
    public const string GlobalPermissionClaimType = "global_permission";

    /// <summary>
    /// Claim type for department-scoped permissions.
    /// Format: "permission_name:department_id"
    /// </summary>
    public const string DepartmentPermissionClaimType = "dept_permission";

    /// <summary>
    /// Claim type for role names.
    /// </summary>
    public const string RoleClaimType = "role";

    /// <summary>
    /// Claim type for department context.
    /// </summary>
    public const string DepartmentIdClaimType = "department_id";

    /// <summary>
    /// Claim type for institute context.
    /// </summary>
    public const string InstituteIdClaimType = "institute_id";

    /// <summary>
    /// Claim type for academic year context.
    /// </summary>
    public const string AcademicYearIdClaimType = "academic_year_id";

    /// <summary>
    /// Claim type for university/tenant ID.
    /// </summary>
    public const string UniversityIdClaimType = "university_id";

    /// <summary>
    /// Policy name prefix for permission-based policies.
    /// </summary>
    public const string PermissionPolicyPrefix = "Permission:";
}
