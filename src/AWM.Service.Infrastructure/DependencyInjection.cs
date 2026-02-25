using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.FileStorage;
using AWM.Service.Infrastructure.Persistence;
using AWM.Service.Infrastructure.Persistence.Interceptors;
using AWM.Service.Infrastructure.Persistence.Repositories.Common;
using AWM.Service.Infrastructure.Persistence.Repositories.Core;
using AWM.Service.Infrastructure.Persistence.Repositories.Defense;
using AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;
using AWM.Service.Infrastructure.Persistence.Repositories.Thesis;
using AWM.Service.Infrastructure.Persistence.Repositories.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AWM.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

            options.UseSqlServer(connectionString, sqlOptions =>
                   {
                       sqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 3,
                           maxRetryDelay: TimeSpan.FromSeconds(10),
                           errorNumbersToAdd: null);
                   })
                   .AddInterceptors(interceptor);
        });

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Common Repositories (Critical)
        services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
        services.AddScoped<IPeriodRepository, PeriodRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();

        // Register Workflow Repository (Critical)
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();

        // Register Core Repositories
        services.AddScoped<IUniversityRepository, UniversityRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();

        // Register Defense Repositories
        services.AddScoped<ICommissionRepository, CommissionRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IPreDefenseAttemptRepository, PreDefenseAttemptRepository>();
        services.AddScoped<IEvaluationCriteriaRepository, EvaluationCriteriaRepository>();
        services.AddScoped<IProtocolRepository, ProtocolRepository>();

        // Register Thesis Repositories
        services.AddScoped<IDirectionRepository, DirectionRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<ITopicApplicationRepository, TopicApplicationRepository>();
        services.AddScoped<IStudentWorkRepository, StudentWorkRepository>();
        services.AddScoped<IReviewerRepository, ReviewerRepository>();
        services.AddScoped<IExpertRepository, ExpertRepository>();
        services.AddScoped<ISupervisorReviewRepository, SupervisorReviewRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // Register Dictionary/Lookup Repositories
        services.AddScoped<IAcademicProgramRepository, AcademicProgramRepository>();
        services.AddScoped<IDegreeLevelRepository, DegreeLevelRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IOrganizationLookupRepository, OrganizationLookupRepository>();

        // Register File Storage Service
        // Switch to S3FileStorageService for production (add AWSSDK.S3 NuGet + configure "FileStorage:S3" section)
        services.AddScoped<IAttachmentService, LocalFileStorageService>();

        return services;
    }
}


