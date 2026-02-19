namespace AWM.Service.Application.Authorization;

using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.Domain.Repositories;

/// <summary>
/// Service that maps roles to their granted permissions.
/// Implements the role-permission matrix for Context-Aware RBAC.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IUserRepository _userRepository;

    public PermissionService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }
    /// <summary>
    /// Static mapping of roles to their permissions.
    /// This is the central definition of what each role can do.
    /// </summary>
    private static readonly Dictionary<string, HashSet<Permission>> RolePermissions = new()
    {
        // ========== Student ==========
        ["Student"] = new()
        {
            // Topics
            Permission.Topics_View,
            Permission.Topics_ViewAvailable,
            
            // Applications
            Permission.Applications_ViewOwn,
            Permission.Applications_Create,
            Permission.Applications_Withdraw,
            
            // Works
            Permission.Works_ViewOwn,
            Permission.Works_EditOwn,
            
            // Quality Checks
            Permission.QualityChecks_Submit,
            
            // Attachments
            Permission.Attachments_Upload,
            Permission.Attachments_Download,
            
            // Notifications
            Permission.Notifications_View,
            Permission.Notifications_MarkRead,
            
            // Academic Programs & Degree Levels (view)
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            
            // Defense (view only)
            Permission.PreDefense_View,
            Permission.Defense_View,
            
            // Organization (view)
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View
        },

        // ========== Supervisor (Scientific Advisor) ==========
        ["Supervisor"] = new()
        {
            // Directions
            Permission.Directions_View,
            Permission.Directions_Create,
            Permission.Directions_Edit,
            Permission.Directions_Submit,
            
            // Topics
            Permission.Topics_View,
            Permission.Topics_Create,
            Permission.Topics_Edit,
            Permission.Topics_Close,
            
            // Applications
            Permission.Applications_View,
            Permission.Applications_Accept,
            Permission.Applications_Reject,
            
            // Works
            Permission.Works_View,
            Permission.Works_ViewSupervised,
            Permission.Works_ChangeState,
            Permission.Works_ManageParticipants,
            
            // Reviews
            Permission.Reviews_View,
            Permission.Reviews_CreateSupervisor,
            
            // Quality Checks
            Permission.QualityChecks_View,
            
            // Attachments
            Permission.Attachments_Download,
            
            // Academic Programs & Degree Levels (view)
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            
            // Defense (view)
            Permission.PreDefense_View,
            Permission.Defense_View,
            
            // Organization (view)
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View,
            
            // Notifications
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },

        // ========== Head of Department ==========
        ["HeadOfDepartment"] = new()
        {
            // Directions (approval)
            Permission.Directions_View,
            Permission.Directions_Approve,
            Permission.Directions_Reject,
            Permission.Directions_RequestRevision,
            
            // Topics (approval)
            Permission.Topics_View,
            Permission.Topics_Approve,
            Permission.Topics_Close,
            
            // Applications (view)
            Permission.Applications_View,
            
            // Works
            Permission.Works_View,
            Permission.Works_ChangeState,
            
            // Commissions
            Permission.Commissions_View,
            Permission.Commissions_Manage,
            Permission.Commissions_ManageMembers,
            
            // Reviews
            Permission.Reviews_View,
            
            // Quality Checks
            Permission.QualityChecks_View,
            
            // Pre-Defense
            Permission.PreDefense_View,
            Permission.PreDefense_Schedule,
            
            // Defense
            Permission.Defense_View,
            Permission.Defense_Schedule,
            
            // Department management
            Permission.Department_Manage,
            Permission.Periods_View,
            Permission.Periods_Manage,
            Permission.Institutes_View,
            Permission.Departments_View,
            
            // Academic Programs & Degree Levels
            Permission.AcademicPrograms_View,
            Permission.AcademicPrograms_Create,
            Permission.AcademicPrograms_Edit,
            Permission.DegreeLevels_View,
            Permission.DegreeLevels_Create,
            
            // Staff & Students
            Permission.Staff_View,
            Permission.Staff_Create,
            Permission.Staff_Edit,
            Permission.Students_View,
            Permission.Students_Create,
            Permission.Students_Edit,
            
            // Users (view in department)
            Permission.Users_View,
            Permission.Roles_Manage,
            
            // Reports
            Permission.Reports_View,
            Permission.Reports_Export,
            Permission.Statistics_ViewDepartment,
            
            // Attachments
            Permission.Attachments_Download,
            
            // Notifications
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },

        // ========== Secretary ==========
        ["Secretary"] = new()
        {
            // Directions (view)
            Permission.Directions_View,
            
            // Topics (view)
            Permission.Topics_View,
            
            // Applications (view)
            Permission.Applications_View,
            
            // Works
            Permission.Works_View,
            
            // Commissions (view)
            Permission.Commissions_View,
            
            // Pre-Defense
            Permission.PreDefense_View,
            Permission.PreDefense_Schedule,
            Permission.PreDefense_RecordAttendance,
            Permission.PreDefense_Finalize,
            
            // Defense
            Permission.Defense_View,
            Permission.Defense_Schedule,
            Permission.Defense_AssignSlot,
            Permission.Defense_Finalize,
            Permission.Defense_GenerateProtocol,
            
            // Reviews
            Permission.Reviews_View,
            Permission.Reviews_UploadExternal,
            
            // Academic Programs & Degree Levels (view)
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            
            // Organization (view)
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View,
            
            // Staff & Students (view)
            Permission.Staff_View,
            Permission.Students_View,
            
            // Attachments
            Permission.Attachments_Upload,
            Permission.Attachments_Download,
            
            // Reports
            Permission.Reports_View,
            Permission.Reports_Export,
            
            // Notifications
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },

        // ========== Expert (Quality Control) ==========
        ["Expert"] = new()
        {
            // Quality Checks
            Permission.QualityChecks_View,
            Permission.QualityChecks_Perform,
            
            // Works (view for checking)
            Permission.Works_View,
            
            // Attachments
            Permission.Attachments_Download,
            
            // Notifications
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },

        // ========== Commission Member ==========
        ["CommissionMember"] = new()
        {
            // Pre-Defense
            Permission.PreDefense_View,
            Permission.PreDefense_Grade,
            
            // Defense
            Permission.Defense_View,
            Permission.Defense_Grade,
            
            // Works (view for grading)
            Permission.Works_View,
            
            // Reviews
            Permission.Reviews_View,
            
            // Attachments
            Permission.Attachments_Download,
            
            // Notifications
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },

        // ========== Vice-Rector ==========
        ["ViceRector"] = new()
        {
            // View everything
            Permission.Directions_View,
            Permission.Topics_View,
            Permission.Applications_View,
            Permission.Works_View,
            Permission.Commissions_View,
            Permission.PreDefense_View,
            Permission.Defense_View,
            Permission.Reviews_View,
            Permission.QualityChecks_View,
            
            // Reports and Statistics
            Permission.Reports_View,
            Permission.Reports_Export,
            Permission.Statistics_ViewDepartment,
            Permission.Statistics_ViewUniversity,
            
            // Academic Programs & Degree Levels (view)
            Permission.AcademicPrograms_View,
            Permission.DegreeLevels_View,
            
            // Organization (view)
            Permission.Periods_View,
            Permission.Institutes_View,
            Permission.Departments_View,
            
            // Staff & Students (view)
            Permission.Staff_View,
            Permission.Students_View,
            
            // Users
            Permission.Users_View,
            
            // Attachments
            Permission.Attachments_Download,
            
            // Notifications
            Permission.Notifications_View,
            Permission.Notifications_MarkRead
        },

        // ========== Admin ==========
        ["Admin"] = new(Enum.GetValues<Permission>()) // All permissions
    };

    /// <inheritdoc />
    public IReadOnlySet<Permission> GetPermissionsForRole(string roleName)
    {
        if (RolePermissions.TryGetValue(roleName, out var permissions))
        {
            return permissions;
        }

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
        var user = await _userRepository.GetWithRoleAssignmentsAsync(userId, cancellationToken);

        if (user == null)
        {
            return new HashSet<Permission>();
        }

        var permissions = new HashSet<Permission>();

        foreach (var assignment in user.RoleAssignments.Where(ra => ra.IsCurrentlyValid()))
        {
            if (assignment.Role != null)
            {
                var rolePermissions = GetPermissionsForRole(assignment.Role.SystemName);
                permissions.UnionWith(rolePermissions);
            }
        }

        return permissions;
    }

    /// <inheritdoc />
    public async Task<IReadOnlySet<Permission>> GetUserPermissionsInContextAsync(
        int userId,
        int? departmentId,
        int? academicYearId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithRoleAssignmentsAsync(userId, cancellationToken);

        if (user == null)
        {
            return new HashSet<Permission>();
        }

        var permissions = new HashSet<Permission>();

        foreach (var assignment in user.RoleAssignments.Where(ra => ra.AppliesToContext(departmentId, academicYearId)))
        {
            if (assignment.Role != null)
            {
                var rolePermissions = GetPermissionsForRole(assignment.Role.SystemName);
                permissions.UnionWith(rolePermissions);
            }
        }

        return permissions;
    }
}
