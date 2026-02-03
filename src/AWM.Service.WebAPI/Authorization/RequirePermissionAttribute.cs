namespace AWM.Service.WebAPI.Authorization;

using AWM.Service.Domain.Auth.Enums;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Attribute for declarative permission-based authorization.
/// Usage: [RequirePermission(Permission.Topics_Create)]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Creates a new authorization attribute requiring the specified permission.
    /// </summary>
    /// <param name="permission">The permission required to access the resource.</param>
    public RequirePermissionAttribute(Permission permission)
        : base(policy: $"{AuthorizationConstants.PermissionPolicyPrefix}{permission}")
    {
    }
}

/// <summary>
/// Attribute for department-context permission authorization.
/// The department ID must be present in the route as 'departmentId'.
/// Usage: [RequireDepartmentPermission(Permission.Works_View)]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireDepartmentPermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Creates a new authorization attribute requiring the permission in department context.
    /// </summary>
    /// <param name="permission">The permission required within the department.</param>
    public RequireDepartmentPermissionAttribute(Permission permission)
        : base(policy: $"{AuthorizationConstants.PermissionPolicyPrefix}{permission}:Department")
    {
    }
}
