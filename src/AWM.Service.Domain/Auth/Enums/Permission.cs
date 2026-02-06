namespace AWM.Service.Domain.Auth.Enums;

/// <summary>
/// Granular permissions for Context-Aware RBAC.
/// Permissions define what actions a user can perform.
/// </summary>
public enum Permission
{
    // ========== Directions ==========
    /// <summary>View directions in department.</summary>
    Directions_View,
    /// <summary>Create new directions (Supervisor).</summary>
    Directions_Create,
    /// <summary>Edit own directions.</summary>
    Directions_Edit,
    /// <summary>Submit direction for review.</summary>
    Directions_Submit,
    /// <summary>Approve directions (HeadOfDepartment).</summary>
    Directions_Approve,
    /// <summary>Reject directions (HeadOfDepartment).</summary>
    Directions_Reject,
    /// <summary>Request revision of direction.</summary>
    Directions_RequestRevision,

    // ========== Topics ==========
    /// <summary>View topics in department.</summary>
    Topics_View,
    /// <summary>View available topics for selection (Student).</summary>
    Topics_ViewAvailable,
    /// <summary>Create new topics (Supervisor).</summary>
    Topics_Create,
    /// <summary>Edit own topics.</summary>
    Topics_Edit,
    /// <summary>Approve topics (HeadOfDepartment).</summary>
    Topics_Approve,
    /// <summary>Close topic.</summary>
    Topics_Close,

    // ========== Topic Applications ==========
    /// <summary>View applications (Supervisor, Admin).</summary>
    Applications_View,
    /// <summary>View own applications (Student).</summary>
    Applications_ViewOwn,
    /// <summary>Create application for topic (Student).</summary>
    Applications_Create,
    /// <summary>Accept application (Supervisor).</summary>
    Applications_Accept,
    /// <summary>Reject application (Supervisor).</summary>
    Applications_Reject,
    /// <summary>Withdraw own application (Student).</summary>
    Applications_Withdraw,

    // ========== Student Works ==========
    /// <summary>View all works in department.</summary>
    Works_View,
    /// <summary>View own work (Student).</summary>
    Works_ViewOwn,
    /// <summary>View supervised works (Supervisor).</summary>
    Works_ViewSupervised,
    /// <summary>Edit works.</summary>
    Works_Edit,
    /// <summary>Edit own work (Student).</summary>
    Works_EditOwn,
    /// <summary>Change work state (workflow transition).</summary>
    Works_ChangeState,
    /// <summary>Add/remove participants.</summary>
    Works_ManageParticipants,

    // ========== Quality Checks ==========
    /// <summary>View quality checks.</summary>
    QualityChecks_View,
    /// <summary>Perform quality check (Expert).</summary>
    QualityChecks_Perform,
    /// <summary>Submit work for check (Student).</summary>
    QualityChecks_Submit,

    // ========== Reviews ==========
    /// <summary>View reviews.</summary>
    Reviews_View,
    /// <summary>Create supervisor review.</summary>
    Reviews_CreateSupervisor,
    /// <summary>Upload external review.</summary>
    Reviews_UploadExternal,

    // ========== Pre-Defense ==========
    /// <summary>View pre-defense schedule.</summary>
    PreDefense_View,
    /// <summary>Schedule pre-defense (Secretary).</summary>
    PreDefense_Schedule,
    /// <summary>Record attendance (Secretary).</summary>
    PreDefense_RecordAttendance,
    /// <summary>Submit grade (Commission Member).</summary>
    PreDefense_Grade,
    /// <summary>Finalize pre-defense (Secretary).</summary>
    PreDefense_Finalize,

    // ========== Final Defense ==========
    /// <summary>View defense schedule.</summary>
    Defense_View,
    /// <summary>Schedule defense (Secretary).</summary>
    Defense_Schedule,
    /// <summary>Assign work to slot.</summary>
    Defense_AssignSlot,
    /// <summary>Submit grade (GAK Member).</summary>
    Defense_Grade,
    /// <summary>Finalize defense (Secretary).</summary>
    Defense_Finalize,
    /// <summary>Generate protocol.</summary>
    Defense_GenerateProtocol,

    // ========== Commissions ==========
    /// <summary>View commissions.</summary>
    Commissions_View,
    /// <summary>Create/manage commissions.</summary>
    Commissions_Manage,
    /// <summary>Add/remove commission members.</summary>
    Commissions_ManageMembers,

    // ========== Attachments ==========
    /// <summary>Upload attachments to work.</summary>
    Attachments_Upload,
    /// <summary>Download attachments.</summary>
    Attachments_Download,
    /// <summary>Delete attachments.</summary>
    Attachments_Delete,

    // ========== Notifications ==========
    /// <summary>View own notifications.</summary>
    Notifications_View,
    /// <summary>Mark notifications as read.</summary>
    Notifications_MarkRead,

    // ========== User Management ==========
    /// <summary>View users.</summary>
    Users_View,
    /// <summary>Create users.</summary>
    Users_Create,
    /// <summary>Edit users.</summary>
    Users_Edit,
    /// <summary>Deactivate users.</summary>
    Users_Deactivate,
    /// <summary>Manage role assignments.</summary>
    Roles_Manage,

    // ========== Organization Management ==========
    /// <summary>Manage department settings.</summary>
    Department_Manage,
    /// <summary>Manage institute settings.</summary>
    Institute_Manage,
    /// <summary>Manage periods.</summary>
    Periods_Manage,

    // ========== Reports ==========
    /// <summary>View reports.</summary>
    Reports_View,
    /// <summary>Export reports.</summary>
    Reports_Export,
    /// <summary>View department statistics.</summary>
    Statistics_ViewDepartment,
    /// <summary>View university-wide statistics.</summary>
    Statistics_ViewUniversity
}
