namespace AWM.Service.Application.Authorization;

using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Auth.Interfaces;

/// <summary>
/// Service that maps roles to their granted permissions.
/// Implements the role-permission matrix for Context-Aware RBAC.
/// </summary>
public class PermissionService : IPermissionService
{
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
            
            // Defense (view only)
            Permission.PreDefense_View,
            Permission.Defense_View
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
            
            // Defense (view)
            Permission.PreDefense_View,
            Permission.Defense_View,
            
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
            Permission.Periods_Manage,
            
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
    public Task<IReadOnlySet<Permission>> GetUserPermissionsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        // Note: This method requires user repository access.
        // In real implementation, inject IUserRepository and load user's role assignments.
        // For now, return empty set as placeholder.
        throw new NotImplementedException(
            "This method should be called through the infrastructure layer with repository access.");
    }

    /// <inheritdoc />
    public Task<IReadOnlySet<Permission>> GetUserPermissionsInContextAsync(
        int userId,
        int? departmentId,
        int? academicYearId,
        CancellationToken cancellationToken = default)
    {
        // Note: This method requires user repository access.
        throw new NotImplementedException(
            "This method should be called through the infrastructure layer with repository access.");
    }
}
