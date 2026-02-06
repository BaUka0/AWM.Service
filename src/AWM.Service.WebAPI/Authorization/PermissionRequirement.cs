namespace AWM.Service.WebAPI.Authorization;

using AWM.Service.Domain.Auth.Enums;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Authorization requirement that checks for a specific permission.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The permission that is required.
    /// </summary>
    public Permission Permission { get; }

    /// <summary>
    /// If true, the permission must be valid for the department context from the request.
    /// If false, only global permissions are checked.
    /// </summary>
    public bool RequireDepartmentContext { get; }

    public PermissionRequirement(Permission permission, bool requireDepartmentContext = false)
    {
        Permission = permission;
        RequireDepartmentContext = requireDepartmentContext;
    }
}
