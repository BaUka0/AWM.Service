namespace AWM.Service.WebAPI.Authorization;

using AWM.Service.Application.Authorization;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Extension methods for configuring Context-Aware RBAC authorization services.
/// </summary>
public static class AuthorizationServiceExtensions
{
    /// <summary>
    /// Adds Context-Aware RBAC authorization services to the DI container.
    /// </summary>
    public static IServiceCollection AddContextAwareAuthorization(this IServiceCollection services)
    {
        // Register permission service
        services.AddScoped<IPermissionService, PermissionService>();

        // Register authorization context
        services.AddScoped<IAuthorizationContext, HttpAuthorizationContext>();

        // Register claims transformation
        services.AddScoped<IClaimsTransformation, ContextClaimsTransformation>();

        // Register authorization handler
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        return services;
    }

    /// <summary>
    /// Configures authorization policies for all permissions.
    /// </summary>
    public static IServiceCollection AddPermissionPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Require authentication by default for all endpoints.
            // Endpoints explicitly marked with [AllowAnonymous] will bypass this.
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Create a policy for each permission
            foreach (Permission permission in Enum.GetValues<Permission>())
            {
                var policyName = $"{AuthorizationConstants.PermissionPolicyPrefix}{permission}";
                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new PermissionRequirement(permission, requireDepartmentContext: false));
                });

                // Create department-context policy
                var deptPolicyName = $"{AuthorizationConstants.PermissionPolicyPrefix}{permission}:Department";
                options.AddPolicy(deptPolicyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new PermissionRequirement(permission, requireDepartmentContext: true));
                });

                // Create institute-context policy
                var instPolicyName = $"{AuthorizationConstants.PermissionPolicyPrefix}{permission}:Institute";
                options.AddPolicy(instPolicyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new PermissionRequirement(permission, requireInstituteContext: true));
                });
            }

            // Add role-based policies for convenience
            options.AddPolicy("RequireStudent", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Student");
            });

            options.AddPolicy("RequireSupervisor", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Supervisor");
            });

            options.AddPolicy("RequireHeadOfDepartment", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("HeadOfDepartment");
            });

            options.AddPolicy("RequireSecretary", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Secretary");
            });

            options.AddPolicy("RequireExpert", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Expert");
            });

            options.AddPolicy("RequireAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });

            options.AddPolicy("RequireCommissionMember", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("CommissionMember");
            });
        });

        return services;
    }
}
