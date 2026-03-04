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
    private readonly ILogger<PermissionHandler> _logger;

    public PermissionHandler(IHttpContextAccessor httpContextAccessor, ILogger<PermissionHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
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
                        _logger.LogInformation("Authorization succeeded: User has permission {Permission} in Dept {DepartmentId}",
                            permissionClaimValue, departmentId);
                        context.Succeed(requirement);
                    }
                    // Also check global permission (applies to all departments)
                    else if (context.User.HasClaim(AuthorizationConstants.GlobalPermissionClaimType, permissionClaimValue))
                    {
                        _logger.LogInformation("Authorization succeeded: User has global permission {Permission} (applies to Dept {DepartmentId})",
                            permissionClaimValue, departmentId);
                        context.Succeed(requirement);
                    }
                    else
                    {
                        _logger.LogWarning("Authorization failed: User HAS {Permission} claim but NOT for Dept {DepartmentId}",
                           permissionClaimValue, departmentId);
                    }
                }
                else
                {
                    _logger.LogWarning("Authorization failed: Requirement for {Permission} depends on Dept ID, but no 'departmentId' found in request",
                        permissionClaimValue);
                }
            }
            else if (requirement.RequireInstituteContext)
            {
                // Get institute ID from route or query
                var instituteId = GetInstituteIdFromRequest();

                if (instituteId.HasValue)
                {
                    // Check if user has permission in this specific institute
                    var institutePermissionClaim = $"{permissionClaimValue}:{instituteId}";
                    if (context.User.HasClaim(AuthorizationConstants.InstitutePermissionClaimType, institutePermissionClaim))
                    {
                        _logger.LogInformation("Authorization succeeded: User has permission {Permission} in Institute {InstituteId}",
                            permissionClaimValue, instituteId);
                        context.Succeed(requirement);
                    }
                    // Also check global permission (applies to all institutes)
                    else if (context.User.HasClaim(AuthorizationConstants.GlobalPermissionClaimType, permissionClaimValue))
                    {
                        _logger.LogInformation("Authorization succeeded: User has global permission {Permission} (applies to Institute {InstituteId})",
                            permissionClaimValue, instituteId);
                        context.Succeed(requirement);
                    }
                    else
                    {
                        _logger.LogWarning("Authorization failed: User HAS {Permission} claim but NOT for Institute {InstituteId}",
                           permissionClaimValue, instituteId);
                    }
                }
                else
                {
                    _logger.LogWarning("Authorization failed: Requirement for {Permission} depends on Institute ID, but no 'instituteId' found in request",
                        permissionClaimValue);
                }
            }
            else
            {
                // Global permission check passed
                _logger.LogInformation("Authorization succeeded: User has required permission {Permission}", permissionClaimValue);
                context.Succeed(requirement);
            }
        }
        else
        {
            _logger.LogTrace("User does not have permission claim {Permission}", permissionClaimValue);
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

        // Try to get from user claims (fallback for when front-end doesn't send it but user has context)
        var claimValue = httpContext.User.FindFirst(AuthorizationConstants.DepartmentIdClaimType)?.Value;
        if (!string.IsNullOrEmpty(claimValue) && int.TryParse(claimValue, out var claimDeptId))
        {
            return claimDeptId;
        }

        return null;
    }

    private int? GetInstituteIdFromRequest()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        // Try to get from route
        if (httpContext.Request.RouteValues.TryGetValue("instituteId", out var routeValue) &&
            int.TryParse(routeValue?.ToString(), out var instId))
        {
            return instId;
        }

        // Try to get from query string
        if (httpContext.Request.Query.TryGetValue("instituteId", out var queryValue) &&
            int.TryParse(queryValue.FirstOrDefault(), out var queryInstId))
        {
            return queryInstId;
        }

        // Try to get from user claims
        var claimValue = httpContext.User.FindFirst(AuthorizationConstants.InstituteIdClaimType)?.Value;
        if (!string.IsNullOrEmpty(claimValue) && int.TryParse(claimValue, out var claimInstId))
        {
            return claimInstId;
        }

        return null;
    }
}
