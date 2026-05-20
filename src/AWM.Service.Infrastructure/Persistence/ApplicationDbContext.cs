namespace AWM.Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

// Common
using AWM.Service.Domain.CommonDomain.Entities;

// Auth
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.ViewModels;

// Wf
using AWM.Service.Domain.Wf.Entities;

// Thesis
using AWM.Service.Domain.Thesis.Entities;

// Defense
using AWM.Service.Domain.Defense.Entities;

/// <summary>
/// Main application DbContext for EF Core.
/// Contains addon entity DbSets organized by schema.
/// University master data is in separate UniversityDbContext (read-only).
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region Common Schema
    public DbSet<WorkflowStage> WorkflowStages => Set<WorkflowStage>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<Notification> Notifications => Set<Notification>();
    #endregion

    #region Auth Schema
    // RBAC+ entities
    public DbSet<RoleAccess> RoleAccesses => Set<RoleAccess>();
    public DbSet<RoleOperation> RoleOperations => Set<RoleOperation>();
    public DbSet<RoleActionType> RoleActionTypes => Set<RoleActionType>();
    public DbSet<RoleOperationAction> RoleOperationActions => Set<RoleOperationAction>();
    public DbSet<UserAccess> UserAccesses => Set<UserAccess>();
    public DbSet<UserAccessHistory> UserAccessHistories => Set<UserAccessHistory>();
    public DbSet<LocalAccount> LocalAccounts => Set<LocalAccount>();
    #endregion

    #region Wf Schema
    public DbSet<WorkType> WorkTypes => Set<WorkType>();
    public DbSet<State> States => Set<State>();
    public DbSet<Transition> Transitions => Set<Transition>();
    #endregion

    #region Thesis Schema
    public DbSet<Direction> Directions => Set<Direction>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<TopicApplication> TopicApplications => Set<TopicApplication>();
    public DbSet<StudentWork> StudentWorks => Set<StudentWork>();
    public DbSet<WorkParticipant> WorkParticipants => Set<WorkParticipant>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<WorkflowHistory> WorkflowHistories => Set<WorkflowHistory>();
    public DbSet<QualityCheck> QualityChecks => Set<QualityCheck>();
    public DbSet<Expert> Experts => Set<Expert>();
    public DbSet<Reviewer> Reviewers => Set<Reviewer>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<SupervisorReview> SupervisorReviews => Set<SupervisorReview>();

    // Reference tables (replacing enums)
    public DbSet<ApplicationStatus> ApplicationStatuses => Set<ApplicationStatus>();
    public DbSet<AttachmentType> AttachmentTypes => Set<AttachmentType>();
    public DbSet<CheckType> CheckTypes => Set<CheckType>();
    #endregion

    #region Defense Schema
    public DbSet<Commission> Commissions => Set<Commission>();
    public DbSet<CommissionMember> CommissionMembers => Set<CommissionMember>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<PreDefenseAttempt> PreDefenseAttempts => Set<PreDefenseAttempt>();
    public DbSet<EvaluationCriteria> EvaluationCriteria => Set<EvaluationCriteria>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Protocol> Protocols => Set<Protocol>();

    // Reference tables (replacing enums)
    public DbSet<CommissionType> CommissionTypes => Set<CommissionType>();
    public DbSet<CommissionRole> CommissionRoles => Set<CommissionRole>();
    public DbSet<AttendanceStatus> AttendanceStatuses => Set<AttendanceStatus>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // RBAC+ Database Views (HasNoKey)
        modelBuilder.Entity<UserAccessMatrix>().HasNoKey().ToView("UserAccessMatrix", "Auth");
        modelBuilder.Entity<RoleAccessMatrix>().HasNoKey().ToView("RoleAccessMatrix", "Auth");
        modelBuilder.Entity<ReducedUserAccessMatrix>().HasNoKey().ToView("ReducedUserAccessMatrix", "Auth");
    }
}
