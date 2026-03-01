namespace AWM.Service.WebAPI.Authorization;

using System.Security.Claims;
using AWM.Service.Application.Authorization;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;

using Microsoft.Extensions.Logging;

/// <summary>
/// Claims transformation that loads user permissions from the database
/// and adds them as claims for authorization.
/// </summary>
public class ContextClaimsTransformation : IClaimsTransformation
{
    private readonly IUserRepository _userRepository;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<ContextClaimsTransformation> _logger;

    public ContextClaimsTransformation(
        IUserRepository userRepository,
        IPermissionService permissionService,
        ILogger<ContextClaimsTransformation> logger)
    {
        _userRepository = userRepository;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Only transform if user is authenticated
        if (principal.Identity?.IsAuthenticated != true)
            return principal;

        // Get user ID from claims
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return principal;

        // Check if already transformed (avoid double transformation)
        if (principal.HasClaim(c => c.Type == AuthorizationConstants.PermissionClaimType))
            return principal;

        // Load user with role assignments from database
        var user = await _userRepository.GetWithRoleAssignmentsAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found in database during claims transformation", userId);
            return principal;
        }

        _logger.LogInformation("Starting claims transformation for user {UserId} ({Login})", userId, user.Login);
        _logger.LogDebug("User {UserId} has {AssignmentsCount} role assignments", userId, user.RoleAssignments.Count);

        // Create new identity with additional claims
        var identity = new ClaimsIdentity();
        var claims = new List<Claim>();

        // Process each role assignment
        foreach (var assignment in user.RoleAssignments)
        {
            if (!assignment.IsCurrentlyValid())
            {
                _logger.LogTrace("Skipping expired/inactive role assignment {AssignmentId}", assignment.Id);
                continue;
            }

            var roleName = assignment.Role?.SystemName ?? assignment.RoleId.ToString();
            _logger.LogDebug("Processing assignment {AssignmentId}: Role={RoleName}, Dept={DeptId}, Year={YearId}",
                assignment.Id, roleName, assignment.DepartmentId, assignment.AcademicYearId);

            // Add role claim
            claims.Add(new Claim(AuthorizationConstants.RoleClaimType, roleName));

            // Get permissions for this role
            var permissions = _permissionService.GetPermissionsForRole(roleName);

            // Add permission claims based on context
            if (assignment.DepartmentId == null && assignment.AcademicYearId == null)
            {
                // Global assignment - add as global permissions
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim(AuthorizationConstants.PermissionClaimType, permission.ToString()));
                    claims.Add(new Claim(AuthorizationConstants.GlobalPermissionClaimType, permission.ToString()));
                }
            }
            else
            {
                // Context-specific assignment
                foreach (var permission in permissions)
                {
                    // Add general permission claim
                    claims.Add(new Claim(AuthorizationConstants.PermissionClaimType, permission.ToString()));

                    // Add department-scoped permission if applicable
                    if (assignment.DepartmentId.HasValue)
                    {
                        var deptPermission = $"{permission}:{assignment.DepartmentId}";
                        claims.Add(new Claim(AuthorizationConstants.DepartmentPermissionClaimType, deptPermission));
                    }

                    // Add institute-scoped permission if applicable
                    if (assignment.InstituteId.HasValue)
                    {
                        var instPermission = $"{permission}:{assignment.InstituteId}";
                        claims.Add(new Claim(AuthorizationConstants.InstitutePermissionClaimType, instPermission));
                    }
                }

                // Add context claims
                if (assignment.DepartmentId.HasValue)
                {
                    claims.Add(new Claim(AuthorizationConstants.DepartmentIdClaimType,
                        assignment.DepartmentId.Value.ToString()));
                }
                if (assignment.InstituteId.HasValue)
                {
                    claims.Add(new Claim(AuthorizationConstants.InstituteIdClaimType,
                        assignment.InstituteId.Value.ToString()));
                }
                if (assignment.AcademicYearId.HasValue)
                {
                    claims.Add(new Claim(AuthorizationConstants.AcademicYearIdClaimType,
                        assignment.AcademicYearId.Value.ToString()));
                }
            }
        }

        // Remove duplicates
        var distinctClaims = claims.DistinctBy(c => $"{c.Type}:{c.Value}").ToList();
        _logger.LogInformation("Generated {TotalClaims} claims for user {UserId}", distinctClaims.Count, userId);

        identity.AddClaims(distinctClaims);

        // Clone principal and add new identity
        var clonedPrincipal = principal.Clone();
        clonedPrincipal.AddIdentity(identity);

        return clonedPrincipal;
    }
}
