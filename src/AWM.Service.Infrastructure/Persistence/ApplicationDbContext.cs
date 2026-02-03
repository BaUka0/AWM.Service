namespace AWM.Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

// Org
using AWM.Service.Domain.Org.Entities;

// Common
using AWM.Service.Domain.CommonDomain.Entities;

// Auth
using AWM.Service.Domain.Auth.Entities;

// Edu
using AWM.Service.Domain.Edu.Entities;

// Wf
using AWM.Service.Domain.Wf.Entities;

// Thesis
using AWM.Service.Domain.Thesis.Entities;

// Defense
using AWM.Service.Domain.Defense.Entities;

/// <summary>
/// Main application DbContext for EF Core.
/// Contains all entity DbSets organized by schema.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region Org Schema
    public DbSet<University> Universities => Set<University>();
    public DbSet<Institute> Institutes => Set<Institute>();
    public DbSet<Department> Departments => Set<Department>();
    #endregion

    #region Common Schema
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Period> Periods => Set<Period>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<Notification> Notifications => Set<Notification>();
    #endregion

    #region Auth Schema
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();
    #endregion

    #region Edu Schema
    public DbSet<DegreeLevel> DegreeLevels => Set<DegreeLevel>();
    public DbSet<AcademicProgram> AcademicPrograms => Set<AcademicProgram>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Staff> Staff => Set<Staff>();
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
    #endregion

    #region Defense Schema
    public DbSet<Commission> Commissions => Set<Commission>();
    public DbSet<CommissionMember> CommissionMembers => Set<CommissionMember>();
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
    }
}
