namespace AWM.Service.WebAPI.Authorization;

using System.Security.Claims;
using AWM.Service.Domain.Auth.Enums;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Authorization handler that checks permissions from claims.
/// Supports both global permissions and department-context permissions.
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Check if user has the permission claim
        var permissionClaimValue = requirement.Permission.ToString();
        var hasPermission = context.User.HasClaim(AuthorizationConstants.PermissionClaimType, permissionClaimValue);

        if (hasPermission)
        {
            if (requirement.RequireDepartmentContext)
            {
                // Get department ID from route or query
                var departmentId = GetDepartmentIdFromRequest();
                
                if (departmentId.HasValue)
                {
                    // Check if user has permission in this specific department
                    var departmentPermissionClaim = $"{permissionClaimValue}:{departmentId}";
                    if (context.User.HasClaim(AuthorizationConstants.DepartmentPermissionClaimType, departmentPermissionClaim))
                    {
                        context.Succeed(requirement);
                    }
                    // Also check global permission (applies to all departments)
                    else if (context.User.HasClaim(AuthorizationConstants.GlobalPermissionClaimType, permissionClaimValue))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            else
            {
                // Global permission check passed
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }

    private int? GetDepartmentIdFromRequest()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        // Try to get from route
        if (httpContext.Request.RouteValues.TryGetValue("departmentId", out var routeValue) &&
            int.TryParse(routeValue?.ToString(), out var deptId))
        {
            return deptId;
        }

        // Try to get from query string
        if (httpContext.Request.Query.TryGetValue("departmentId", out var queryValue) &&
            int.TryParse(queryValue.FirstOrDefault(), out var queryDeptId))
        {
            return queryDeptId;
        }

        return null;
    }
}
