namespace AWM.Service.WebAPI.Authorization;

using System.Security.Claims;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Auth.Interfaces;

/// <summary>
/// Implementation of IAuthorizationContext that reads from HttpContext claims.
/// Provides context-aware permission checking for the current request.
/// </summary>
public class HttpAuthorizationContext : IAuthorizationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private IReadOnlySet<Permission>? _cachedPermissions;
    private Dictionary<int, HashSet<Permission>>? _cachedDepartmentPermissions;

    public HttpAuthorizationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <inheritdoc />
    public int? UserId
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    /// <inheritdoc />
    public int? UniversityId
    {
        get
        {
            var claim = User?.FindFirst(AuthorizationConstants.UniversityIdClaimType)
                     ?? User?.FindFirst("UniversityId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    /// <inheritdoc />
    public int? CurrentDepartmentId
    {
        get
        {
            // Try to get from route first
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request.RouteValues.TryGetValue("departmentId", out var routeValue) == true &&
                int.TryParse(routeValue?.ToString(), out var deptId))
            {
                return deptId;
            }

            // Fall back to query
            if (httpContext?.Request.Query.TryGetValue("departmentId", out var queryValue) == true &&
                int.TryParse(queryValue.FirstOrDefault(), out var queryDeptId))
            {
                return queryDeptId;
            }

            return null;
        }
    }

    /// <inheritdoc />
    public int? CurrentInstituteId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request.RouteValues.TryGetValue("instituteId", out var routeValue) == true &&
                int.TryParse(routeValue?.ToString(), out var instId))
            {
                return instId;
            }
            return null;
        }
    }

    /// <inheritdoc />
    public int? CurrentAcademicYearId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request.RouteValues.TryGetValue("academicYearId", out var routeValue) == true &&
                int.TryParse(routeValue?.ToString(), out var yearId))
            {
                return yearId;
            }

            if (httpContext?.Request.Query.TryGetValue("academicYearId", out var queryValue) == true &&
                int.TryParse(queryValue.FirstOrDefault(), out var queryYearId))
            {
                return queryYearId;
            }

            return null;
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public IReadOnlyList<string> Roles
    {
        get
        {
            if (User == null) return Array.Empty<string>();

            return User.FindAll(AuthorizationConstants.RoleClaimType)
                .Concat(User.FindAll(ClaimTypes.Role))
                .Select(c => c.Value)
                .Distinct()
                .ToList();
        }
    }

    /// <inheritdoc />
    public bool HasPermission(Permission permission)
    {
        return GetAllPermissions().Contains(permission);
    }

    /// <inheritdoc />
    public bool HasPermissionInDepartment(Permission permission, int departmentId)
    {
        return GetPermissionsInDepartment(departmentId).Contains(permission);
    }

    /// <inheritdoc />
    public bool HasPermissionInContext(Permission permission, int? departmentId, int? academicYearId)
    {
        // Check global permission first
        var globalPermissions = User?.FindAll(AuthorizationConstants.GlobalPermissionClaimType)
            .Select(c => c.Value)
            .ToHashSet() ?? new HashSet<string>();

        if (globalPermissions.Contains(permission.ToString()))
            return true;

        // If department context provided, check department-scoped permission
        if (departmentId.HasValue)
        {
            return GetPermissionsInDepartment(departmentId.Value).Contains(permission);
        }

        // Fall back to any permission
        return HasPermission(permission);
    }

    /// <inheritdoc />
    public bool HasAnyPermission(params Permission[] permissions)
    {
        var allPermissions = GetAllPermissions();
        return permissions.Any(p => allPermissions.Contains(p));
    }

    /// <inheritdoc />
    public bool HasAllPermissions(params Permission[] permissions)
    {
        var allPermissions = GetAllPermissions();
        return permissions.All(p => allPermissions.Contains(p));
    }

    /// <inheritdoc />
    public IReadOnlySet<Permission> GetAllPermissions()
    {
        if (_cachedPermissions != null)
            return _cachedPermissions;

        var permissions = new HashSet<Permission>();

        if (User == null)
        {
            _cachedPermissions = permissions;
            return permissions;
        }

        // Get all permission claims
        var permissionClaims = User.FindAll(AuthorizationConstants.PermissionClaimType);

        foreach (var claim in permissionClaims)
        {
            if (Enum.TryParse<Permission>(claim.Value, out var permission))
            {
                permissions.Add(permission);
            }
        }

        _cachedPermissions = permissions;
        return permissions;
    }

    /// <inheritdoc />
    public IReadOnlySet<Permission> GetPermissionsInDepartment(int departmentId)
    {
        _cachedDepartmentPermissions ??= new Dictionary<int, HashSet<Permission>>();

        if (_cachedDepartmentPermissions.TryGetValue(departmentId, out var cached))
            return cached;

        var permissions = new HashSet<Permission>();

        if (User == null)
        {
            _cachedDepartmentPermissions[departmentId] = permissions;
            return permissions;
        }

        // Add global permissions (apply everywhere)
        var globalClaims = User.FindAll(AuthorizationConstants.GlobalPermissionClaimType);
        foreach (var claim in globalClaims)
        {
            if (Enum.TryParse<Permission>(claim.Value, out var permission))
            {
                permissions.Add(permission);
            }
        }

        // Add department-specific permissions
        var deptClaims = User.FindAll(AuthorizationConstants.DepartmentPermissionClaimType);
        foreach (var claim in deptClaims)
        {
            var parts = claim.Value.Split(':');
            if (parts.Length == 2 &&
                int.TryParse(parts[1], out var claimDeptId) &&
                claimDeptId == departmentId &&
                Enum.TryParse<Permission>(parts[0], out var permission))
            {
                permissions.Add(permission);
            }
        }

        _cachedDepartmentPermissions[departmentId] = permissions;
        return permissions;
    }
}
