namespace AWM.Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.ViewModels;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Infrastructure.Persistence.Interceptors;
using AWM.Service.Domain.University;

public class ApplicationDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableInterceptor) : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
    }

    #region University Schema (Read-Only)
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<OrgUnit> OrgUnits => Set<OrgUnit>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<EmployeePosition> EmployeePositions => Set<EmployeePosition>();
    public DbSet<Speciality> Specialities => Set<Speciality>();
    public DbSet<Semester> Semesters => Set<Semester>();
    #endregion

    #region Auth Schema (RBAC+)
    public DbSet<LocalAccount> LocalAccounts => Set<LocalAccount>();
    public DbSet<RoleOperation> RoleOperations => Set<RoleOperation>();
    public DbSet<RoleOperationAction> RoleOperationActions => Set<RoleOperationAction>();
    public DbSet<RoleActionType> RoleActionTypes => Set<RoleActionType>();
    public DbSet<RoleAccess> RoleAccesses => Set<RoleAccess>();
    public DbSet<UserAccess> UserAccesses => Set<UserAccess>();
    public DbSet<UserAccessHistory> UserAccessHistories => Set<UserAccessHistory>();
    #endregion

    #region Common Schema
    public DbSet<WorkflowStage> WorkflowStages => Set<WorkflowStage>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<StaffAssignment> StaffAssignments => Set<StaffAssignment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    #endregion

    #region Workflow (Wf) Schema
    public DbSet<WorkType> WorkTypes => Set<WorkType>();
    public DbSet<State> States => Set<State>();
    public DbSet<Transition> Transitions => Set<Transition>();
    #endregion

    #region Thesis Schema
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<TopicApplication> TopicApplications => Set<TopicApplication>();
    public DbSet<StudentWork> StudentWorks => Set<StudentWork>();
    public DbSet<WorkParticipant> WorkParticipants => Set<WorkParticipant>();
    public DbSet<WorkflowHistory> WorkflowHistory => Set<WorkflowHistory>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<QualityCheck> QualityChecks => Set<QualityCheck>();
    public DbSet<WorkReview> WorkReviews => Set<WorkReview>();
    public DbSet<Reviewer> Reviewers => Set<Reviewer>();
    public DbSet<Direction> Directions => Set<Direction>();

    // Reference tables
    public DbSet<AttachmentType> AttachmentTypes => Set<AttachmentType>();
    public DbSet<CheckType> CheckTypes => Set<CheckType>();
    public DbSet<SpecialityCheckType> SpecialityCheckTypes => Set<SpecialityCheckType>();
    #endregion

    #region Defense Schema
    public DbSet<Commission> Commissions => Set<Commission>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<PreDefenseAttempt> PreDefenseAttempts => Set<PreDefenseAttempt>();
    public DbSet<EvaluationCriteria> EvaluationCriteria => Set<EvaluationCriteria>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Protocol> Protocols => Set<Protocol>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Fix 4.3: Global Query Filters for ISoftDeletable entities.
        // Automatically exclude soft-deleted records from all queries.
        // Use .IgnoreQueryFilters() in repositories when you need to include deleted records.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Domain.Common.ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Domain.Common.ISoftDeletable.IsDeleted));
                var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        // RBAC+ Database Views (HasNoKey)
        modelBuilder.Entity<UserAccessMatrix>().HasNoKey().ToView("UserAccessMatrix", "Auth");
        modelBuilder.Entity<RoleAccessMatrix>().HasNoKey().ToView("RoleAccessMatrix", "Auth");
        modelBuilder.Entity<ReducedUserAccessMatrix>().HasNoKey().ToView("ReducedUserAccessMatrix", "Auth");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor);
    }
}
