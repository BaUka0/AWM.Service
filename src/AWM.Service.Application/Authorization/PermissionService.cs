namespace AWM.Service.Application.Authorization;

using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Repositories;

using Microsoft.Extensions.Logging;

/// <summary>
/// Service that maps roles to their granted permissions.
/// Implements the role-permission matrix for Context-Aware RBAC.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(IUserRepository userRepository, ILogger<PermissionService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    /// Static mapping of roles to their permissions.
    /// This is the central definition of what each role can do.
    /// </summary>
    private static readonly Dictionary<string, HashSet<Permission>> RolePermissions = new()
    {
        [nameof(RoleType.Student)] = new()
        {
            Permission.Topics_View,
            Permission.Topics_ViewAvailable,
            Permission.Applications_ViewOwn,
            Permission.Applications_Create,
            Permission.Applications_Withdraw,
            Permission.Works_ViewOwn,
            Permission.Works_EditOwn,
            Permission.QualityChecks_Submit,
            Permission.Attachments_Upload,
            Permission.Attachments_Download,
            Permission.Notifications_View,
            Permission.Notifications_MarkRead,
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            Permission.PreDefense_View,
            Permission.Defense_View,
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View
        },
        [nameof(RoleType.Supervisor)] = new()
        {
            Permission.Directions_View,
            Permission.Directions_Create,
            Permission.Directions_Edit,
            Permission.Directions_Submit,
            Permission.Topics_View,
            Permission.Topics_Create,
            Permission.Topics_Edit,
            Permission.Topics_Close,
            Permission.Applications_View,
            Permission.Applications_Accept,
            Permission.Applications_Reject,
            Permission.Works_View,
            Permission.Works_ViewSupervised,
            Permission.Works_ChangeState,
            Permission.Works_ManageParticipants,
            Permission.Reviews_View,
            Permission.Reviews_CreateSupervisor,
            Permission.QualityChecks_View,
            Permission.Attachments_Download,
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            Permission.PreDefense_View,
            Permission.Defense_View,
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View,
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },
        [nameof(RoleType.HeadOfDepartment)] = new()
        {
            Permission.Directions_View,
            Permission.Directions_Approve,
            Permission.Directions_Reject,
            Permission.Directions_RequestRevision,
            Permission.Topics_View,
            Permission.Topics_Approve,
            Permission.Topics_Close,
            Permission.Applications_View,
            Permission.Works_View,
            Permission.Works_ChangeState,
            Permission.Commissions_View,
            Permission.Commissions_Manage,
            Permission.Commissions_ManageMembers,
            Permission.Reviews_View,
            Permission.QualityChecks_View,
            Permission.PreDefense_View,
            Permission.PreDefense_Schedule,
            Permission.Defense_View,
            Permission.Defense_Schedule,
            Permission.Department_Manage,
            Permission.Periods_View,
            Permission.Periods_Manage,
            Permission.Institutes_View,
            Permission.Departments_View,
            Permission.AcademicPrograms_View,
            Permission.AcademicPrograms_Create,
            Permission.AcademicPrograms_Edit,
            Permission.DegreeLevels_View,
            Permission.DegreeLevels_Create,
            Permission.Staff_View,
            Permission.Staff_Create,
            Permission.Staff_Edit,
            Permission.Students_View,
            Permission.Students_Create,
            Permission.Students_Edit,
            Permission.Users_View,
            Permission.Roles_Manage,
            Permission.Reports_View,
            Permission.Reports_Export,
            Permission.Statistics_ViewDepartment,
            Permission.Attachments_Download,
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },
        [nameof(RoleType.Secretary)] = new()
        {
            Permission.Directions_View,
            Permission.Topics_View,
            Permission.Applications_View,
            Permission.Works_View,
            Permission.Commissions_View,
            Permission.PreDefense_View,
            Permission.PreDefense_Schedule,
            Permission.PreDefense_RecordAttendance,
            Permission.PreDefense_Finalize,
            Permission.Defense_View,
            Permission.Defense_Schedule,
            Permission.Defense_AssignSlot,
            Permission.Defense_Finalize,
            Permission.Defense_GenerateProtocol,
            Permission.Reviews_View,
            Permission.Reviews_UploadExternal,
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View,
            Permission.Staff_View,
            Permission.Students_View,
            Permission.Attachments_Upload,
            Permission.Attachments_Download,
            Permission.Reports_View,
            Permission.Reports_Export,
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },
        [nameof(RoleType.Expert)] = new()
        {
            Permission.QualityChecks_View,
            Permission.QualityChecks_Perform,
            Permission.Works_View,
            Permission.Attachments_Download,
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },
        [nameof(RoleType.CommissionMember)] = new()
        {
            Permission.PreDefense_View,
            Permission.PreDefense_Grade,
            Permission.Defense_View,
            Permission.Defense_Grade,
            Permission.Works_View,
            Permission.Reviews_View,
            Permission.Attachments_Download,
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },
        [nameof(RoleType.ViceRector)] = new()
        {
            Permission.Directions_View,
            Permission.Topics_View,
            Permission.Applications_View,
            Permission.Works_View,
            Permission.Commissions_View,
            Permission.PreDefense_View,
            Permission.Defense_View,
            Permission.Reviews_View,
            Permission.QualityChecks_View,
            Permission.Reports_View,
            Permission.Reports_Export,
            Permission.Statistics_ViewDepartment,
            Permission.Statistics_ViewUniversity,
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View,
            Permission.Staff_View,
            Permission.Students_View,
            Permission.Users_View,
            Permission.Attachments_Download,
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },
        [nameof(RoleType.Admin)] = new(Enum.GetValues<Permission>())
    };

    /// <inheritdoc />
    public IReadOnlySet<Permission> GetPermissionsForRole(string roleName)
    {
        if (RolePermissions.TryGetValue(roleName, out var permissions))
        {
            _logger.LogDebug("Permissions found for role {RoleName}: {PermissionsCount} permissions", roleName, permissions.Count);
            return permissions;
        }

        _logger.LogWarning("No permissions defined for role {RoleName}", roleName);
        return new HashSet<Permission>();
    }

    /// <inheritdoc />
    public bool RoleHasPermission(string roleName, Permission permission)
    {
        return GetPermissionsForRole(roleName).Contains(permission);
    }

    /// <inheritdoc />
    public async Task<IReadOnlySet<Permission>> GetUserPermissionsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting permissions for user {UserId}", userId);

        var user = await _userRepository.GetWithRoleAssignmentsAsync(userId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when fetching permissions", userId);
            return new HashSet<Permission>();
        }

        _logger.LogDebug("User {UserId} has {AssignmentsCount} role assignments", userId, user.RoleAssignments.Count);

        var permissions = new HashSet<Permission>();

        foreach (var assignment in user.RoleAssignments.Where(ra => ra.IsCurrentlyValid()))
        {
            if (assignment.Role != null)
            {
                var roleName = assignment.Role.SystemName;
                var rolePermissions = GetPermissionsForRole(roleName);

                _logger.LogTrace("Adding {PermissionsCount} permissions from role {RoleName} for user {UserId}",
                    rolePermissions.Count, roleName, userId);

                permissions.UnionWith(rolePermissions);
            }
        }

        _logger.LogInformation("User {UserId} has total {TotalPermissionsCount} valid permissions", userId, permissions.Count);
        return permissions;
    }

    /// <inheritdoc />
    public async Task<IReadOnlySet<Permission>> GetUserPermissionsInContextAsync(
        int userId,
        int? departmentId,
        int? academicYearId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking context permissions for user {UserId}. Context: Dept={DepartmentId}, Year={YearId}",
            userId, departmentId, academicYearId);

        var user = await _userRepository.GetWithRoleAssignmentsAsync(userId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when checking context permissions", userId);
            return new HashSet<Permission>();
        }

        var permissions = new HashSet<Permission>();

        foreach (var assignment in user.RoleAssignments)
        {
            bool applies = assignment.AppliesToContext(departmentId, academicYearId);

            _logger.LogTrace("Assignment ID {AssignmentId} (Role={RoleName}): Valid={IsValid}, MatchesContext={Matches}",
                assignment.Id, assignment.Role?.SystemName ?? "Unknown", assignment.IsCurrentlyValid(), applies);

            if (applies && assignment.Role != null)
            {
                var rolePermissions = GetPermissionsForRole(assignment.Role.SystemName);
                permissions.UnionWith(rolePermissions);
            }
        }

        _logger.LogInformation("User {UserId} has {TotalPermissionsCount} permissions in context Dept={DepartmentId}",
            userId, permissions.Count, departmentId);

        return permissions;
    }
}
